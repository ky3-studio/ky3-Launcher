//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Launcher.Web.Hoyolab.Passport;
using System.Text;

namespace Launcher.Web.Hoyolab;

internal sealed partial class Cookie
{
    private readonly SortedDictionary<string, string> inner;

    public Cookie()
        : this([])
    {
    }

    public Cookie(SortedDictionary<string, string> dict)
    {
        inner = dict;
        ApplyV2Aliases(dict);
    }

    public string this[string key]
    {
        get => inner[key];
        set => inner[key] = value;
    }

    public static Cookie Parse(string cookieString)
    {
        SortedDictionary<string, string> cookieMap = [];
        cookieString = cookieString.Replace(" ", string.Empty, StringComparison.Ordinal);
        string[] values = cookieString.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (string[] parts in values.Select(c => c.Split('=', 2)))
        {
            string name = parts[0].Trim();
            string value = parts.Length == 1 ? string.Empty : parts[1].Trim();
            cookieMap.TryAdd(name, value);
        }

        return new(cookieMap);
    }

    public static Cookie FromCoreWebView2Cookies(IReadOnlyList<CoreWebView2Cookie> webView2Cookies)
    {
        SortedDictionary<string, string> cookieMap = [];

        foreach (CoreWebView2Cookie cookie in webView2Cookies)
        {
            cookieMap.TryAdd(cookie.Name, cookie.Value);
        }

        return new(cookieMap);
    }

    public static Cookie FromLoginResult(LoginResult? loginResult)
    {
        if (loginResult is null)
        {
            return new();
        }

        SortedDictionary<string, string> cookieMap = new();
        if (loginResult.UserInfo is not null)
        {
            cookieMap.TryAdd(STUID, loginResult.UserInfo.Aid);
            cookieMap.TryAdd(MID, loginResult.UserInfo.Mid);
        }

        if (loginResult.Token is not null)
        {
            cookieMap.TryAdd(STOKEN, loginResult.Token.Token);
        }

        return new(cookieMap);
    }

    public static Cookie FromQrLoginResult(QrLoginResult? qrLoginResult)
    {
        if (qrLoginResult is null)
        {
            return new();
        }

        SortedDictionary<string, string> cookieMap = new()
        {
            [STUID] = qrLoginResult.UserInfo.Aid,
            [STOKEN] = qrLoginResult.Tokens.Single(token => token.TokenType is 1).Token,
            [MID] = qrLoginResult.UserInfo.Mid,
        };

        return new(cookieMap);
    }

    public static Cookie FromSToken(string stuid, string stoken)
    {
        SortedDictionary<string, string> cookieMap = new()
        {
            [STUID] = stuid,
            [STOKEN] = stoken,
        };

        return new(cookieMap);
    }

    public bool IsEmpty()
    {
        return inner.Count <= 0;
    }

    private static void ApplyV2Aliases(SortedDictionary<string, string> map)
    {
        Alias(map, COOKIE_TOKEN, COOKIE_TOKEN + "_v2");
        Alias(map, LTOKEN, LTOKEN + "_v2");
        Alias(map, ACCOUNT_ID, ACCOUNT_ID + "_v2");
        Alias(map, LTUID, LTUID + "_v2");
        Alias(map, STOKEN, STOKEN + "_v2");
        Alias(map, STUID, STUID + "_v2");
        Alias(map, STUID, ACCOUNT_ID + "_v2");
        Alias(map, MID, "mid_v2");
        Alias(map, MID, "account_mid_v2");
        Alias(map, MID, "ltmid_v2");
    }

    private static void Alias(SortedDictionary<string, string> map, string legacy, string v2)
    {
        if (map.TryGetValue(legacy, out string? current) && !string.IsNullOrEmpty(current))
        {
            return;
        }

        if (map.TryGetValue(v2, out string? value) && !string.IsNullOrEmpty(value))
        {
            map[legacy] = value;
        }
    }

    public bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
    {
        return inner.TryGetValue(key, out value);
    }

    public string? GetValueOrDefault(string key)
    {
        return inner.GetValueOrDefault(key);
    }

    public override string ToString()
    {
        StringBuilder resultBuilder = new();

        IEnumerator<KeyValuePair<string, string>> enumerator = inner.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return string.Empty;
        }

        KeyValuePair<string, string> first = enumerator.Current;
        resultBuilder.Append(first.Key).Append('=').Append(first.Value);

        if (!enumerator.MoveNext())
        {
            return resultBuilder.ToString();
        }

        do
        {
            resultBuilder.Append(';');
            KeyValuePair<string, string> current = enumerator.Current;
            resultBuilder.Append(current.Key).Append('=').Append(current.Value);
        }
        while (enumerator.MoveNext());

        return resultBuilder.ToString();
    }
}
