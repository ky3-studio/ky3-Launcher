//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Logging;
using kyxsan.Model.Entity;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.Service;
using kyxsan.Service.kyxsan;
using kyxsan.Service.Notification;
using Windows.System;

namespace kyxsan.ViewModel.Wiki;

[Service(ServiceLifetime.Scoped)]
internal sealed partial class WikiAvatarStrategyComponent
{
    private readonly IAvatarStrategyService avatarStrategyService;
    private readonly CultureOptions cultureOptions;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial WikiAvatarStrategyComponent(IServiceProvider serviceProvider);

    public bool IsBilibiliAvailable { get => cultureOptions.LocaleName is LocaleNames.CHS; }

    [Command("BilibiliStrategyCommand")]
    private static async Task OpenBilibiliStrategyWebsiteAsync(Avatar? avatar)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Open strategy website", "WikiAvatarStrategyComponent.Command", [("target", "bilibili")]));

        if (avatar is null)
        {
            return;
        }

        Uri targetUri = $"https://wiki.biligame.com/ys/{avatar.Name}/攻略".ToUri();
        await Launcher.LaunchUriAsync(targetUri);
    }

    [Command("ChineseStrategyCommand")]
    private async Task OpenChineseStrategyWebsiteAsync(Avatar? avatar)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Open strategy website", "WikiAvatarStrategyComponent.Command", [("target", "miyoushe")]));

        if (avatar is null)
        {
            return;
        }

        AvatarStrategy? strategy = await avatarStrategyService.GetStrategyByAvatarId(avatar.Id).ConfigureAwait(false);

        if (strategy is null)
        {
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelWikiAvatarStrategyNotFound));
            return;
        }

        Uri targetUri = strategy.ChineseStrategyUrl;
        if (string.IsNullOrEmpty(targetUri.OriginalString))
        {
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelWikiAvatarStrategyNotFound));
            return;
        }

        await Launcher.LaunchUriAsync(targetUri);
    }

    [Command("OverseaStrategyCommand")]
    private async Task OpenOverseaStrategyWebsiteAsync(Avatar? avatar)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Open strategy website", "WikiAvatarStrategyComponent.Command", [("target", "hoyolab")]));

        if (avatar is null)
        {
            return;
        }

        AvatarStrategy? strategy = await avatarStrategyService.GetStrategyByAvatarId(avatar.Id).ConfigureAwait(false);

        if (strategy is null)
        {
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelWikiAvatarStrategyNotFound));
            return;
        }

        Uri targetUri = strategy.OverseaStrategyUrl;
        if (string.IsNullOrEmpty(targetUri.OriginalString))
        {
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelWikiAvatarStrategyNotFound));
            return;
        }

        await Launcher.LaunchUriAsync(targetUri);
    }
}