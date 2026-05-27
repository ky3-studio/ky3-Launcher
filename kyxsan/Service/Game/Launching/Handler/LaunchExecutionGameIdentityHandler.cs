//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DependencyInjection.Abstraction;
using kyxsan.Core.ExceptionService;
using kyxsan.Model.Entity.Primitive;
using kyxsan.Service.Game.Account;
using kyxsan.Service.Game.Launching.Context;
using kyxsan.Service.Game.Scheme;
using kyxsan.Service.User;
using kyxsan.Web.Hoyolab.Passport;
using kyxsan.Web.Hoyolab.Takumi.Binding;
using kyxsan.Web.Response;

namespace kyxsan.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionGameIdentityHandler : AbstractLaunchExecutionHandler
{
    public override async ValueTask BeforeAsync(BeforeLaunchExecutionContext context)
    {
        if (context.LaunchOptions.UsingHoyolabAccount.Value)
        {
            await HandleHoyolabAccountAsync(context).ConfigureAwait(false);
        }
        else if (context.Identity.GameAccount is { } account && !RegistryInterop.Set(account))
        {
            kyxsanException.Throw(SH.ViewModelLaunchGameSwitchGameAccountFail);
        }
    }

    public override async ValueTask AfterAsync(AfterLaunchExecutionContext context)
    {
        LaunchStatusOptions options = context.ServiceProvider.GetRequiredService<LaunchStatusOptions>();
        await context.TaskContext.SwitchToMainThreadAsync();
        options.UserGameRole = default;
    }

    private static async ValueTask HandleHoyolabAccountAsync(BeforeLaunchExecutionContext context)
    {
        if (context.TargetScheme.SchemeType is SchemeType.ChineseBilibili)
        {
            return;
        }

        if (context.Identity.UserAndUid is not { } userAndUid)
        {
            return;
        }

        if (userAndUid.IsOversea ^ context.TargetScheme.IsOversea)
        {
            kyxsanException.NotSupported(SH.ViewModelLaunchGameAccountAndServerNotMatch);
        }

        using (IServiceScope scope = context.ServiceProvider.CreateScope())
        {
            IHoyoPlayPassportClient client = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<IHoyoPlayPassportClient>>()
                .CreateFor(userAndUid);
            Response<AuthTicketWrapper> resp = await client
                .CreateAuthTicketAsync(userAndUid.User)
                .ConfigureAwait(false);

            if (ResponseValidator.TryValidate(resp, scope.ServiceProvider, out AuthTicketWrapper? wrapper))
            {
                IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                UserGameRole? userGameRole = await userService.GetUserGameRoleByUidAsync(userAndUid.Uid.Value).ConfigureAwait(false);

                await context.TaskContext.SwitchToMainThreadAsync();
                scope.ServiceProvider.GetRequiredService<LaunchStatusOptions>().UserGameRole = userGameRole;

                context.SetOption(LaunchExecutionOptionsKey.LoginAuthTicket, wrapper.Ticket);
                return;
            }
        }

        kyxsanException.NotSupported(SH.ViewModelLaunchGameCreateAuthTicketFailed);
    }
}