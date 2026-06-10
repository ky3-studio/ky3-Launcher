//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Core.Logging;
using kyxsan.Service.User;
using kyxsan.UI.Xaml.Data;

namespace kyxsan.Web.Hoyolab.Takumi.Binding;

internal sealed partial class UserGameRole : ObservableObject, IPropertyValuesProvider
{
    [JsonPropertyName("game_biz")]
    public string GameBiz { get; set; } = default!;

    [JsonPropertyName("region")]
    public Region Region { get; set; }

    [JsonPropertyName("game_uid")]
    public string GameUid { get; set; } = default!;

    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = default!;

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("is_chosen")]
    public bool IsChosen { get; set; }

    [JsonPropertyName("region_name")]
    public string RegionName { get; set; } = default!;

    [JsonPropertyName("is_official")]
    public bool IsOfficial { get; set; }

    [JsonIgnore]
    public string Description
    {
        get => $"{RegionName} | Lv.{Level}";
    }

    [JsonIgnore]
    [ObservableProperty]
    public partial string? ProfilePictureIcon { get; set; }

    public static implicit operator PlayerUid(UserGameRole userGameRole)
    {
        return new(userGameRole.GameUid, userGameRole.Region);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Nickname} | {RegionName} | Lv.{Level}";
    }

    [Command("RefreshProfilePictureCommand")]
    private async Task RefreshProfilePictureAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Refresh profile picture", "UserGameRole.Command"));
        await Ioc.Default.GetRequiredService<IUserService>().RefreshProfilePictureAsync(this).ConfigureAwait(false);
    }
}