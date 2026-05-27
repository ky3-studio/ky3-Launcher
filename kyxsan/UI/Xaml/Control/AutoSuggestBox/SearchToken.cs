//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Windows.UI;

namespace kyxsan.UI.Xaml.Control.AutoSuggestBox;

internal sealed class SearchToken
{
    public static readonly SearchToken NotFound = new(SearchTokenKind.None, SH.ControlAutoSuggestBoxNotFoundValue, 0);

    public SearchToken(SearchTokenKind kind, string value, int order, Uri? packageIconUri = null, Uri? iconUri = null, Uri? sideIconUri = null, Color? quality = null)
    {
        Value = value;
        Kind = kind;
        PackageIconUri = packageIconUri;
        IconUri = iconUri;
        SideIconUri = sideIconUri;
        Quality = quality;
        Order = order;
    }

    public SearchTokenKind Kind { get; }

    public string Value { get; }

    public Uri? PackageIconUri { get; }

    public Uri? IconUri { get; }

    public Uri? SideIconUri { get; }

    public Color? Quality { get; }

    public int Order { get; }

    public override string ToString()
    {
        return Value;
    }
}