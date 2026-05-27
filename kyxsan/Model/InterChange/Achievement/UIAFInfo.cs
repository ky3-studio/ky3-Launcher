//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using JetBrains.Annotations;
using kyxsan.Core;

namespace kyxsan.Model.InterChange.Achievement;

// ReSharper disable once InconsistentNaming
internal sealed class UIAFInfo
{
    [JsonPropertyName("export_app")]
    public string? ExportApp { get; init; }

    [JsonPropertyName("export_timestamp")]
    public long? ExportTimestamp { get; init; }

    [JsonIgnore]
    [UsedImplicitly]
    public DateTimeOffset ExportDateTime
    {
        get => UnsafeDateTimeOffset.FromUnixTimeRelaxed(ExportTimestamp, DateTimeOffset.MinValue);
    }

    [JsonPropertyName("export_app_version")]
    public string? ExportAppVersion { get; init; }

    // ReSharper disable once InconsistentNaming
    [JsonPropertyName("uiaf_version")]
    public string? UIAFVersion { get; init; }

    public static UIAFInfo CreateForExport()
    {
        return new()
        {
            ExportTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ExportApp = SH.AppName,
            ExportAppVersion = kyxsanRuntime.Version.ToString(),
            UIAFVersion = UIAF.CurrentVersion,
        };
    }

    public static UIAFInfo CreateForEmbeddedYae()
    {
        return new()
        {
            ExportTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ExportApp = "Embedded Yae",
            ExportAppVersion = "ky3 launcher " + kyxsanRuntime.Version.ToString(),
            UIAFVersion = UIAF.CurrentVersion,
        };
    }
}