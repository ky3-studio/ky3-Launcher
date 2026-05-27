//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Core.DependencyInjection.Abstraction;
using kyxsan.Core.Logging;
using kyxsan.Model.Primitive;
using kyxsan.Service.Metadata;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Service.Notification;
using kyxsan.Service.User;
using kyxsan.ViewModel.User;
using kyxsan.Web.Hoyolab.Takumi.GameRecord;
using kyxsan.Web.Hoyolab.Takumi.GameRecord.Avatar;
using kyxsan.Web.Response;
using System.Collections.ObjectModel;

namespace kyxsan.ViewModel.AvatarProperty;

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

    public ObservableCollection<CharacterView>? Characters
    {
        get;
        set
        {
            SetProperty(ref field, value);
        }
    }

    [ObservableProperty]
    public partial CharacterView? SelectedCharacter { get; set; }

    [ObservableProperty]
    public partial CharacterDetailView? CharacterDetail { get; set; }

    [ObservableProperty]
    public partial bool IsLoadingDetail { get; set; }

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

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
        {
            metadataContext = await metadataService.GetContextAsync<AvatarPropertyMetadataContext>(token).ConfigureAwait(false);
            await LoadCharacterListAsync(userAndUid).ConfigureAwait(false);
        }
        else
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
        }

        return true;
    }

    partial void OnSelectedCharacterChanged(CharacterView? value)
    {
        if (value is not null)
        {
            LoadCharacterDetailAsync(value.Id).SafeForget();
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

            ObservableCollection<CharacterView> characters = [];
            foreach (Character apiChar in wrapper.List)
            {
                if (metadataContext.IdAvatarMap.TryGetValue(apiChar.Id, out Model.Metadata.Avatar.Avatar? metaAvatar))
                {
                    characters.Add(new CharacterView(apiChar, metaAvatar));
                }
            }

            await taskContext.SwitchToMainThreadAsync();
            Characters = characters;

            if (characters.Count > 0)
            {
                SelectedCharacter = characters[0];
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

            CharacterDetailView detailView = new(detail, metaAvatar, metaWeapon);

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
}
