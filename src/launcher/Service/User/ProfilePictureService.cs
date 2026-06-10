//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Entity;
using kyxsan.Model.Intrinsic;
using kyxsan.Service.Metadata;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Web.Enka;
using kyxsan.Web.Enka.Model;
using kyxsan.Web.Hoyolab.Takumi.Binding;

namespace kyxsan.Service.User;

[Service(ServiceLifetime.Singleton, typeof(IProfilePictureService))]
internal sealed partial class ProfilePictureService : IProfilePictureService
{
    private readonly IUidProfilePictureRepository uidProfilePictureRepository;
    private readonly IMetadataService metadataService;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    private readonly Lock syncRoot = new();

    [GeneratedConstructor]
    public partial ProfilePictureService(IServiceProvider serviceProvider);

    public async ValueTask TryInitializeAsync(ViewModel.User.User user, CancellationToken token = default)
    {
        foreach (UserGameRole userGameRole in user.UserGameRoles)
        {
            UidProfilePicture? profilePicture;
            lock (syncRoot)
            {
                profilePicture = uidProfilePictureRepository.SingleUidProfilePictureOrDefaultByUid(userGameRole.GameUid);
            }

            if (profilePicture is not null)
            {
                if (await TryAttachProfilePictureToUserGameRoleAsync(userGameRole, profilePicture, token).ConfigureAwait(false))
                {
                    continue;
                }
            }

            // Force update
            await RefreshUserGameRoleAsync(userGameRole, token).ConfigureAwait(false);
        }
    }

    public async ValueTask RefreshUserGameRoleAsync(UserGameRole userGameRole, CancellationToken token = default)
    {
        EnkaResponse? enkaResponse;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            EnkaClient enkaClient = scope.ServiceProvider.GetRequiredService<EnkaClient>();

            enkaResponse =
                await enkaClient.GetForwardPlayerInfoAsync(userGameRole, token).ConfigureAwait(false) ??
                await enkaClient.GetPlayerInfoAsync(userGameRole, token).ConfigureAwait(false);
        }

        if (enkaResponse is { PlayerInfo.ProfilePicture: { } innerProfilePicture })
        {
            UidProfilePicture profilePicture = UidProfilePicture.From(userGameRole, innerProfilePicture);

            // We don't use DbTransaction here because it's rather complicated
            // to handle transaction over multiple DbContext
            lock (syncRoot)
            {
                uidProfilePictureRepository.DeleteUidProfilePictureByUid(userGameRole.GameUid);
                uidProfilePictureRepository.UpdateUidProfilePicture(profilePicture);
            }

            await TryAttachProfilePictureToUserGameRoleAsync(userGameRole, profilePicture, token).ConfigureAwait(false);
        }
    }

    private async ValueTask<bool> TryAttachProfilePictureToUserGameRoleAsync(UserGameRole userGameRole, UidProfilePicture cache, CancellationToken token = default)
    {
        if (cache.RefreshTime.AddDays(15) < DateTimeOffset.Now)
        {
            return false;
        }

        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        UserMetadataContext context = await metadataService.GetContextAsync<UserMetadataContext>(token).ConfigureAwait(false);

        await taskContext.SwitchToMainThreadAsync();

        // Most common to most rare
        if (cache.ProfilePictureId is not 0U)
        {
            userGameRole.ProfilePictureIcon = context.ProfilePictures
                .Single(p => p.Id == cache.ProfilePictureId)
                .Icon;

            return true;
        }

        if (cache.AvatarId is not 0U)
        {
            userGameRole.ProfilePictureIcon = context.ProfilePictures
                .Single(p => p.UnlockType is ProfilePictureUnlockBy.Avatar && p.UnlockParameter == cache.AvatarId)
                .Icon;

            return true;
        }

        if (cache.CostumeId is not 0U)
        {
            userGameRole.ProfilePictureIcon = context.ProfilePictures
                .Single(p => p.UnlockType is ProfilePictureUnlockBy.Costume && p.UnlockParameter == cache.CostumeId)
                .Icon;

            return true;
        }

        return false;
    }
}