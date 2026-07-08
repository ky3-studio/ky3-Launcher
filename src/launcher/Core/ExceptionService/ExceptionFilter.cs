using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace Launcher.Core.ExceptionService;

internal static class ExceptionFilter
{
    public static bool IsKnownWinUiFrameworkException(Exception exception)
    {
        // WinUI 3 已知 MUI 资源缓存问题，布局层抛出，非应用代码导致
        if (exception is COMException { HResult: unchecked((int)0x80073B01) })
        {
            return true;
        }

        // WinUI 3 / WinRT 布局层文件查找失败，无 .NET 托管栈追踪，非应用代码导致
        // StackTrace is null 是关键安全门：应用层代码引发的 FileNotFoundException 一定有托管栈
        if (exception is FileNotFoundException { HResult: unchecked((int)0x80070002) } && exception.StackTrace is null)
        {
            return true;
        }

        return false;
    }

    public static bool IsAllSafeNetworkException(AggregateException exception)
    {
        return exception.Flatten().InnerExceptions.All(
            inner => inner is TaskCanceledException or HttpRequestException or OperationCanceledException);
    }
}
