//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;

namespace kyxsan.Model.InterChange.Achievement;

// ReSharper disable once InconsistentNaming
internal sealed class UIAFItem
{
    [JsonPropertyName("id")]
    public uint Id { get; init; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; init; }

    [JsonPropertyName("current")]
    public uint Current { get; init; }

    [JsonPropertyName("status")]
    public AchievementStatus Status { get; init; }

    public static UIAFItem From(Entity.Achievement source)
    {
        return new()
        {
            Id = source.Id,
            Current = source.Current,
            Status = source.Status,
            Timestamp = source.Time.ToUnixTimeSeconds(),
        };
    }
}