//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.IO;
using kyxsan.Factory.IO;
using kyxsan.Service.Game;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace kyxsan.Service.GachaLog.QueryProvider;

[Service(ServiceLifetime.Transient, typeof(IGachaLogQueryProvider), Key = RefreshOptionKind.LogFile)]
internal sealed partial class GachaLogQueryLogFileProvider : IGachaLogQueryProvider
{
    private readonly IMemoryStreamFactory memoryStreamFactory;
    private readonly ITaskContext taskContext;
    private readonly CultureOptions cultureOptions;

    [GeneratedConstructor]
    public partial GachaLogQueryLogFileProvider(IServiceProvider serviceProvider);

    [GeneratedRegex(@".:(?:\\|/).+(?:GenshinImpact|YuanShen)(?=_Data)", RegexOptions.IgnoreCase)]
    private static partial Regex GamePathRegex { get; }

    public async ValueTask<ValueResult<bool, GachaLogQuery>> GetQueryAsync()
    {
        await taskContext.SwitchToBackgroundAsync();

        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        string[] logPaths =
        [
            Path.Combine(appDataPath, @"..\LocalLow\miHoYo\原神\output_log.txt"),
            Path.Combine(appDataPath, @"..\LocalLow\miHoYo\Genshin Impact\output_log.txt"),
        ];

        foreach (string logPath in logPaths)
        {
            if (!File.Exists(logPath))
            {
                continue;
            }

            string? gamePath = await FindGamePathFromLogAsync(logPath).ConfigureAwait(false);
            if (string.IsNullOrEmpty(gamePath))
            {
                continue;
            }

            string cacheFile = GachaLogQueryWebCacheProvider.GetCacheFile(gamePath);
            if (!File.Exists(cacheFile))
            {
                continue;
            }

            bool isOversea = cacheFile.Contains(GameConstants.GenshinImpactData, StringComparison.OrdinalIgnoreCase);

            TempFileStream fileStream;
            try
            {
                fileStream = TempFileStream.CopyFrom(cacheFile, FileMode.Open, FileAccess.Read);
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }

            using (fileStream)
            {
                using MemoryStream memoryStream = await memoryStreamFactory.GetStreamAsync(fileStream).ConfigureAwait(false);
                string? result = GachaLogQueryWebCacheProvider.MatchUrlFromCacheStream(memoryStream, isOversea);

                if (string.IsNullOrEmpty(result))
                {
                    continue;
                }

                NameValueCollection query = HttpUtility.ParseQueryString(result.TrimEnd("#/log"));
                string? queryLanguageCode = query["lang"];

                if (!LocaleNames.LanguageCodeFitsCurrentLocale(queryLanguageCode, cultureOptions.LocaleName))
                {
                    string message = SH.FormatServiceGachaLogUrlProviderUrlLanguageNotMatchCurrentLocale(queryLanguageCode, cultureOptions.LanguageCode);
                    return new(false, GachaLogQuery.Invalid(message));
                }

                return new(true, new(result));
            }
        }

        return new(false, GachaLogQuery.Invalid(SH.ServiceGachaLogUrlProviderLogFileUrlNotFound));
    }

    private static async ValueTask<string?> FindGamePathFromLogAsync(string logFilePath)
    {
        string content;
        try
        {
            content = await File.ReadAllTextAsync(logFilePath).ConfigureAwait(false);
        }
        catch (IOException)
        {
            return null;
        }

        if (GamePathRegex.Match(content) is not { Success: true } match)
        {
            return null;
        }

        string fullPath = Path.GetFullPath($"{match.Value}.exe");
        return File.Exists(fullPath) ? fullPath : null;
    }
}
