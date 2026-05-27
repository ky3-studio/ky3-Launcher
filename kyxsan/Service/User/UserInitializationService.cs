//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DependencyInjection.Abstraction;
using kyxsan.Model.Entity.Extension;
using kyxsan.UI.Xaml.Data;
using kyxsan.Web.Hoyolab;
using kyxsan.Web.Hoyolab.Bbs.User;
using kyxsan.Web.Hoyolab.Passport;
using kyxsan.Web.Hoyolab.Takumi.Binding;
using kyxsan.Web.Response;

namespace kyxsan.Service.User;

[Service(ServiceLifetime.Singleton, typeof(IUserInitializationService))]
internal sealed partial class UserInitializationService : IUserInitializationService
{
    private readonly IUserFingerprintService userFingerprintService;
    private readonly IProfilePictureService profilePictureService;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial UserInitializationService(IServiceProvider serviceProvider);

    public ValueTask<ViewModel.User.User> ResumeUserAsync(Model.Entity.User entity, CancellationToken token = default)
    {
        ViewModel.User.User user = ViewModel.User.User.From(entity, serviceProvider);
        return ResumeUserAsync(user, token);
    }

    public async ValueTask<ViewModel.User.User> ResumeUserAsync(ViewModel.User.User user, CancellationToken token = default)
    {
        if (!await InitializeUserAsync(user, token).ConfigureAwait(false))
        {
            user.UserInfo = new()
            {
                Uid = SH.ModelBindingUserInitializationFailed,
                Nickname = SH.ModelBindingUserInitializationFailed,
            };

            await taskContext.SwitchToMainThreadAsync();
            user.UserGameRoles = new AdvancedCollectionView<UserGameRole>([]);
        }

        return user;
    }

    public async ValueTask<ViewModel.User.User?> CreateUserFromInputCookieOrDefaultAsync(InputCookie inputCookie, CancellationToken token = default)
    {
        // 这里只负责创建实体用户，稍后在用户服务中保存到数据库
        (Cookie cookie, bool isOversea, string? deviceFp) = inputCookie;
        Model.Entity.User entity = Model.Entity.User.From(cookie);

        entity.Aid = cookie.GetValueOrDefault(Cookie.STUID);
        entity.Mid = cookie.GetValueOrDefault(Cookie.MID);
        entity.IsOversea = isOversea;
        entity.TryUpdateFingerprint(deviceFp);

        if (entity.Aid is not null && entity.Mid is not null)
        {
            ViewModel.User.User user = ViewModel.User.User.From(entity, serviceProvider);
            bool initialized = await InitializeUserAsync(user, token).ConfigureAwait(false);

            return initialized ? user : null;
        }

        return null;
    }

    private static async ValueTask<bool> TrySetUserLTokenAsync(IServiceProvider serviceProvider, ViewModel.User.User user, CancellationToken token)
    {
        if (user.LToken is not null)
        {
            return true;
        }

        IPassportClient passportClient = serviceProvider
            .GetRequiredService<IOverseaSupportFactory<IPassportClient>>()
            .Create(user.IsOversea);

        Response<LTokenWrapper> lTokenResponse = await passportClient
            .GetLTokenBySTokenAsync(user.Entity, token)
            .ConfigureAwait(false);

        if (ResponseValidator.TryValidate(lTokenResponse, serviceProvider, out LTokenWrapper? wrapper))
        {
            user.LToken = new()
            {
                [Cookie.LTUID] = user.Entity.Aid ?? string.Empty,
                [Cookie.LTOKEN] = wrapper.LToken,
            };
            return true;
        }

        return false;
    }

    private static async ValueTask<bool> TrySetUserCookieTokenAsync(IServiceProvider serviceProvider, ViewModel.User.User user, CancellationToken token)
    {
        if (user.Entity.CookieTokenLastUpdateTime > DateTimeOffset.UtcNow - TimeSpan.FromDays(1))
        {
            if (user.CookieToken is not null)
            {
                return true;
            }
        }

        IPassportClient passportClient = serviceProvider
            .GetRequiredService<IOverseaSupportFactory<IPassportClient>>()
            .Create(user.IsOversea);

        Response<UidCookieToken> cookieTokenResponse = await passportClient
            .GetCookieAccountInfoBySTokenAsync(user.Entity, token)
            .ConfigureAwait(false);

        if (ResponseValidator.TryValidate(cookieTokenResponse, serviceProvider, out UidCookieToken? uidCookieToken))
        {
            user.CookieToken = new()
            {
                [Cookie.ACCOUNT_ID] = user.Entity.Aid ?? string.Empty,
                [Cookie.COOKIE_TOKEN] = uidCookieToken.CookieToken,
            };

            user.Entity.CookieTokenLastUpdateTime = DateTimeOffset.UtcNow;
            user.NeedDbUpdateAfterResume = true;
            return true;
        }

        return false;
    }

    private static async ValueTask<bool> TrySetUserUserInfoAsync(IServiceProvider serviceProvider, ViewModel.User.User user, CancellationToken token)
    {
        IUserClient userClient = serviceProvider
            .GetRequiredService<IOverseaSupportFactory<IUserClient>>()
            .Create(user.IsOversea);

        Response<UserFullInfoWrapper> response = await userClient
            .GetUserFullInfoAsync(user.Entity, token)
            .ConfigureAwait(false);

        if (ResponseValidator.TryValidate(response, serviceProvider, out UserFullInfoWrapper? wrapper))
        {
            user.UserInfo = wrapper.UserInfo;
            return true;
        }

        return false;
    }

    private static async ValueTask<bool> TrySetUserUserGameRolesAsync(IServiceProvider serviceProvider, ViewModel.User.User user, CancellationToken token)
    {
        BindingClient bindingClient = serviceProvider.GetRequiredService<BindingClient>();

        Response<ListWrapper<UserGameRole>> userGameRolesResponse = await bindingClient
            .GetUserGameRolesOverseaAwareAsync(user.Entity, token)
            .ConfigureAwait(false);

        if (ResponseValidator.TryValidate(userGameRolesResponse, serviceProvider, out ListWrapper<UserGameRole>? wrapper))
        {
            user.UserGameRoles = wrapper.List.AsAdvancedCollectionView();
            return true;
        }

        return false;
    }

    private async ValueTask<bool> InitializeUserAsync(ViewModel.User.User user, CancellationToken token = default)
    {
        if (user.IsInitialized)
        {
            return true;
        }

        if (user.SToken is null)
        {
            return false;
        }

        using CancellationTokenSource timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(token);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(30));
        CancellationToken ct = timeoutCts.Token;

        try
        {
            using IServiceScope scopeL = serviceProvider.CreateScope();
            using IServiceScope scopeC = serviceProvider.CreateScope();
            using IServiceScope scopeU = serviceProvider.CreateScope();

            Task<bool> lTokenTask = TrySetUserLTokenAsync(scopeL.ServiceProvider, user, ct).AsTask();
            Task<bool> cookieTokenTask = TrySetUserCookieTokenAsync(scopeC.ServiceProvider, user, ct).AsTask();
            Task<bool> userInfoTask = TrySetUserUserInfoAsync(scopeU.ServiceProvider, user, ct).AsTask();
            Task<bool> gameRolesTask = FetchGameRolesAfterLTokenAsync(lTokenTask, user, ct);

            bool[] results = await Task.WhenAll(lTokenTask, cookieTokenTask, userInfoTask, gameRolesTask).ConfigureAwait(false);

            if (!results[0] || !results[1] || !results[2] || !results[3])
            {
                return false;
            }
        }
        catch (OperationCanceledException)
        {
            return false;
        }

        user.IsInitialized = true;
        _ = InitializeNonCriticalAsync(user, token);
        return true;
    }

    private async Task<bool> FetchGameRolesAfterLTokenAsync(Task<bool> lTokenPrerequisite, ViewModel.User.User user, CancellationToken ct)
    {
        if (!await lTokenPrerequisite.ConfigureAwait(false))
        {
            return false;
        }

        using IServiceScope scope = serviceProvider.CreateScope();
        return await TrySetUserUserGameRolesAsync(scope.ServiceProvider, user, ct).ConfigureAwait(false);
    }

    private async Task InitializeNonCriticalAsync(ViewModel.User.User user, CancellationToken token)
    {
        try
        {
            using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            CancellationToken ct = cts.Token;

            Task fpTask = userFingerprintService.TryInitializeAsync(user, ct).AsTask();
            Task profileTask = profilePictureService.TryInitializeAsync(user, ct).AsTask();
            await Task.WhenAll(fpTask, profileTask).ConfigureAwait(false);
        }
        catch
        {
        }
    }
}