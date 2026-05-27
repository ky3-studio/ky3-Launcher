//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Text.Json.Annotation;
using kyxsan.Core.Text.Json.Converter;
using kyxsan.Model.Intrinsic;

namespace kyxsan.Service.GachaLog.UIGF;

internal sealed class UIGFRoot
{
    [JsonRequired]
    [JsonPropertyName("info")]
    public UIGFInfo Info { get; set; } = default!;

    [JsonPropertyName("hk4e")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<UIGFEntry>? Hk4e { get; set; }

    [JsonPropertyName("list")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<UIGFItem>? List { get; set; }
}

internal sealed class UIGFInfo
{
    [JsonPropertyName("export_timestamp")]
    public long ExportTimestamp { get; set; }

    [JsonPropertyName("export_app")]
    public string ExportApp { get; set; } = default!;

    [JsonPropertyName("export_app_version")]
    public string ExportAppVersion { get; set; } = default!;

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("uigf_version")]
    public string? UIGFVersion { get; set; }

    [JsonPropertyName("uid")]
    public string? Uid { get; set; }

    [JsonPropertyName("lang")]
    public string? Lang { get; set; }

    [JsonPropertyName("region_time_zone")]
    public int? RegionTimeZone { get; set; }
}

internal sealed class UIGFEntry
{
    [JsonPropertyName("uid")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public uint Uid { get; set; }

    [JsonPropertyName("timezone")]
    public int Timezone { get; set; } = 8;

    [JsonPropertyName("lang")]
    public string Language { get; set; } = "zh-cn";

    [JsonPropertyName("list")]
    public List<UIGFItem> List { get; set; } = [];
}

internal sealed class UIGFItem
{
    [JsonPropertyName("uigf_gacha_type")]
    [JsonEnumHandling(JsonEnumHandling.NumberString)]
    public GachaType UIGFGachaType { get; set; }

    [JsonPropertyName("gacha_type")]
    [JsonEnumHandling(JsonEnumHandling.NumberString)]
    public GachaType GachaType { get; set; }

    [JsonPropertyName("item_id")]
    public string ItemId { get; set; } = default!;

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("item_type")]
    public string? ItemType { get; set; }

    [JsonPropertyName("rank_type")]
    public string? RankType { get; set; }

    [JsonPropertyName("count")]
    public string? Count { get; set; }

    [JsonPropertyName("time")]
    [JsonConverter(typeof(SimpleDateTimeConverter))]
    public DateTime Time { get; set; }

    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long Id { get; set; }
}
