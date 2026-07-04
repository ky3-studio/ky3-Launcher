using Launcher.ViewModel.User;

namespace Launcher.Service.AutoSignIn;

internal interface IAutoSignInService
{
    ValueTask<bool> RunAsync(CancellationToken token = default);

    ValueTask<bool> OnUserAndUidChangedAsync(UserAndUid userAndUid, CancellationToken token = default);
}
