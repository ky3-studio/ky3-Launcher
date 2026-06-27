//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.Collections;
using Launcher.Core.DependencyInjection.Abstraction;
using Launcher.Core.Logging;
using Launcher.Core.Setting;
using Launcher.Model;
using Launcher.Model.Primitive;
using Launcher.Service;
using Launcher.Service.Metadata;
using Launcher.Service.Metadata.ContextAbstraction;
using Launcher.Service.Notification;
using Launcher.Service.User;
using Launcher.UI.Xaml.Control.AutoSuggestBox;
using Launcher.UI.Xaml.Data;
using Launcher.ViewModel.User;
using Launcher.Web.Hoyolab.Takumi.GameRecord;
using Launcher.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Launcher.Web.Response;
using System.Collections.Immutable;
using System.Globalization;

namespace Launcher.ViewModel.AvatarProperty;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class AvatarPropertyViewModel : Abstraction.ViewModel, IRecipient<UserAndUidChangedMessage>
{
    private readonly IOverseaSupportFactory<IGameRecordClient> gameRecordClientFactory;
    private readonly IMetadataService metadataService;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;
    private readonly IMessenger messenger;

    private AvatarPropertyMetadataContext? metadataContext;

    [GeneratedConstructor]
    public partial AvatarPropertyViewModel(IServiceProvider serviceProvider);

    public IAdvancedCollectionView<CharacterView>? Characters
    {
        get;
        set
        {
            SetProperty(ref field, value);
        }
    }

    [ObservableProperty]
    public partial CharacterDetailView? CharacterDetail { get; set; }

    [ObservableProperty]
    public partial bool IsLoadingDetail { get; set; }

    [ObservableProperty]
    public partial SearchData? SearchData { get; set; }

    public string FormattedTotalAvatarCount { get => SH.FormatViewModelAvatarPropertyTotalAvatarCountHint(Characters?.Count ?? 0); }

    public ImmutableArray<NameValue<AvatarPropertySortDescriptionKind>> SortDescriptionKinds { get; } = ImmutableCollectionsNameValue.FromEnum<AvatarPropertySortDescriptionKind>(static type => type.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture) ?? string.Empty);

    public NameValue<AvatarPropertySortDescriptionKind>? SortDescriptionKind
    {
        get => field ??= Selection.Initialize(SortDescriptionKinds, UnsafeLocalSetting.Get(SettingKeys.AvatarPropertySortDescriptionKind, AvatarPropertySortDescriptionKind.Default));
        set
        {
            if (value is not null && SetProperty(ref field, value))
            {
                UnsafeLocalSetting.Set(SettingKeys.AvatarPropertySortDescriptionKind, value.Value);
                PrivateSortCharacters();
            }
        }
    }

    public void Receive(UserAndUidChangedMessage message)
    {
        if (message.UserAndUid is { } userAndUid)
        {
            LoadCharacterListAsync(userAndUid).SafeForget();
        }
        else
        {
            Characters = null;
            CharacterDetail = null;
        }
    }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        metadataContext = await metadataService.GetContextAsync<AvatarPropertyMetadataContext>(token).ConfigureAwait(false);
        SearchData searchData = SearchData.CreateForAvatarProperty();

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
        {
            await LoadCharacterListAsync(userAndUid).ConfigureAwait(false);
        }
        else
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
        }

        await taskContext.SwitchToMainThreadAsync();
        SearchData = searchData;

        return true;
    }

    private void OnCurrentCharacterChanged(object? sender, object e)
    {
        if (Characters?.CurrentItem is { } current)
        {
            LoadCharacterDetailAsync(current.Id).SafeForget();
        }
    }

    [Command("RefreshCommand")]
    private async Task RefreshAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Refresh avatar property", "AvatarPropertyViewModel.Command"));

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
        {
            await LoadCharacterListAsync(userAndUid).ConfigureAwait(false);
        }
    }

    [SuppressMessage("", "SH003")]
    private async Task LoadCharacterListAsync(UserAndUid userAndUid)
    {
        if (metadataContext is null)
        {
            return;
        }

        try
        {
            IGameRecordClient gameRecordClient = gameRecordClientFactory.Create(userAndUid.IsOversea);

            Response<ListWrapper<Character>> response;
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                response = await gameRecordClient
                    .GetCharacterListAsync(userAndUid)
                    .ConfigureAwait(false);
            }

            if (!ResponseValidator.TryValidate(response, serviceProvider, out ListWrapper<Character>? wrapper))
            {
                return;
            }

            List<CharacterView> characters = [];
            foreach (Character apiChar in wrapper.List)
            {
                if (apiChar.Weapon is null)
                {
                    continue;
                }

                if (metadataContext.IdAvatarMap.TryGetValue(apiChar.Id, out Model.Metadata.Avatar.Avatar? metaAvatar))
                {
                    metadataContext.IdWeaponMap.TryGetValue(apiChar.Weapon.Id, out Model.Metadata.Weapon.Weapon? metaWeapon);
                    characters.Add(new CharacterView(apiChar, metaAvatar, metaWeapon));
                }
            }

            // Batch-fetch detail for all characters to get skills
            Response<ListWrapper<DetailedCharacter>> detailResponse;
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                detailResponse = await gameRecordClient
                    .GetCharacterDetailAsync(userAndUid, wrapper.List.SelectAsArray(static c => c.Id))
                    .ConfigureAwait(false);
            }

            if (ResponseValidator.TryValidate(detailResponse, serviceProvider, out ListWrapper<DetailedCharacter>? detailWrapper))
            {
                foreach (DetailedCharacter detail in detailWrapper.List)
                {
                    if (detail.Base is null)
                    {
                        continue;
                    }

                    CharacterView? characterView = characters.Find(c => c.Id == detail.Base.Id);
                    if (characterView is not null && metadataContext.IdAvatarMap.TryGetValue(detail.Base.Id, out Model.Metadata.Avatar.Avatar? detailMetaAvatar))
                    {
                        characterView.SetSkills(detail.Skills, detailMetaAvatar);
                    }
                }
            }

            await taskContext.SwitchToMainThreadAsync();

            if (Characters is not null)
            {
                Characters.CurrentChanged -= OnCurrentCharacterChanged;
            }

            IAdvancedCollectionView<CharacterView> view = characters.AsAdvancedCollectionView();
            Characters = view;
            view.CurrentChanged += OnCurrentCharacterChanged;
            OnPropertyChanged(nameof(FormattedTotalAvatarCount));
            PrivateSortCharacters();

            if (characters.Count > 0)
            {
                view.MoveCurrentToFirst();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    [SuppressMessage("", "SH003")]
    private async Task LoadCharacterDetailAsync(AvatarId avatarId)
    {
        if (metadataContext is null)
        {
            return;
        }

        try
        {
            await taskContext.SwitchToMainThreadAsync();
            IsLoadingDetail = true;

            UserAndUid? userAndUid = await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);
            if (userAndUid is null)
            {
                return;
            }

            IGameRecordClient gameRecordClient = gameRecordClientFactory.Create(userAndUid.IsOversea);

            Response<ListWrapper<DetailedCharacter>> response;
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                response = await gameRecordClient
                    .GetCharacterDetailAsync(userAndUid, [avatarId])
                    .ConfigureAwait(false);
            }

            if (!ResponseValidator.TryValidate(response, serviceProvider, out ListWrapper<DetailedCharacter>? wrapper))
            {
                return;
            }

            DetailedCharacter? detail = wrapper.List.FirstOrDefault();
            if (detail is null)
            {
                return;
            }

            if (!metadataContext.IdAvatarMap.TryGetValue(detail.Base.Id, out Model.Metadata.Avatar.Avatar? metaAvatar))
            {
                return;
            }

            if (!metadataContext.IdWeaponMap.TryGetValue(detail.Weapon.Id, out Model.Metadata.Weapon.Weapon? metaWeapon))
            {
                return;
            }

            CharacterDetailView detailView = new(detail, metaAvatar, metaWeapon, metadataContext);

            await taskContext.SwitchToMainThreadAsync();
            CharacterDetail = detailView;
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            await taskContext.SwitchToMainThreadAsync();
            IsLoadingDetail = false;
        }
    }

    private void PrivateSortCharacters()
    {
        ArgumentNullException.ThrowIfNull(SortDescriptionKind);
        if (Characters is not { } characters)
        {
            return;
        }

        using (characters.DeferRefresh())
        {
            characters.SortDescriptions.Clear();
            foreach (ref readonly SortDescription sd in AvatarPropertySortDescriptions.Get(SortDescriptionKind.Value).AsSpan())
            {
                characters.SortDescriptions.Add(sd);
            }
        }

        characters.MoveCurrentToFirst();
    }

    [Command("FilterCommand")]
    private void ApplyFilter()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Filter", "AvatarPropertyViewModel.Command"));

        if (Characters is null)
        {
            return;
        }

        Characters.Filter = CharacterViewFilter.Compile(SearchData);
        OnPropertyChanged(nameof(FormattedTotalAvatarCount));

        if (Characters.CurrentItem is null)
        {
            Characters.MoveCurrentToFirst();
        }
    }
}
