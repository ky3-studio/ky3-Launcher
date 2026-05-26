//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Endpoint.kyxsan;

internal static class StaticResourcesEndpoints
{
    private static readonly string[] ServerRoots =
    [
        "http://8.134.75.17",
    ];

    public static Uri UIIconNone { get; } = StaticRaw("Bg", "UI_Icon_None.png").ToUri();

    public static Uri UIItemIconNone { get; } = StaticRaw("Bg", "UI_ItemIcon_None.png").ToUri();

    public static Uri UIAvatarIconSideNone { get; } = StaticRaw("AvatarIcon", "UI_AvatarIcon_Side_None.png").ToUri();

    public static Uri UIAvatarIconCostumeCard { get; } = StaticRaw("AvatarCard", "UI_AvatarIcon_Costume_Card.png").ToUri();

    public static string StaticRaw(string category, string fileName)
    {
        return string.Intern($"{ServerRoots[0]}/static/raw/{category}/{fileName}");
    }

    public static IEnumerable<Uri> GetFallbackUris(Uri original)
    {
        string originalAuthority = original.GetLeftPart(UriPartial.Authority);
        foreach (string root in ServerRoots)
        {
            if (!root.Equals(originalAuthority, StringComparison.OrdinalIgnoreCase))
            {
                yield return new Uri(original.OriginalString.Replace(originalAuthority, root));
            }
        }
    }
}
