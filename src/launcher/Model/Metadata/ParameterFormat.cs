//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Model.Metadata;

internal sealed class ParameterFormat : IFormatProvider, ICustomFormatter
{
    private static readonly Lazy<ParameterFormat> LazyFormat = new();

    public static string FormatInvariant(string str, object param)
    {
        return string.Format(LazyFormat.Value, str, param);
    }

    [SuppressMessage("", "CA1305")]
    public string Format(string? fmt, object? arg, IFormatProvider? formatProvider)
    {
        ReadOnlySpan<char> fmtSpan = fmt;
        switch (fmtSpan.Length)
        {
            case 3: // FnP
                return string.Format($"{{0:P{fmtSpan[1]}}}", arg);
            case 2: // Fn
                return string.Format($"{{0:{fmtSpan}}}", arg);
            case 1: // P I
                switch (fmtSpan[0])
                {
                    case 'P':
                        return string.Format($"{{0:P0}}", arg);
                    case 'I':
                        return arg is null ? "0" : ((IConvertible)arg).ToInt32(default).ToString();
                }

                break;
        }

        return arg?.ToString() ?? string.Empty;
    }

    public object? GetFormat(Type? formatType)
    {
        return formatType == typeof(ICustomFormatter)
            ? this
            : null;
    }
}