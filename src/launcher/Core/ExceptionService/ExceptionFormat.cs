//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections;
using System.Globalization;
using System.Text;

namespace kyxsan.Core.ExceptionService;

internal static class ExceptionFormat
{
    private const string SectionSeparator = "----------------------------------------";

    public static string Format(Exception exception)
    {
        return Format(new(), exception).ToString();
    }

    public static StringBuilder Format(StringBuilder builder, Exception exception)
    {
        if (exception.Data.Count > 0)
        {
            builder.AppendLine("Exception Data:");

            foreach (DictionaryEntry entry in exception.Data)
            {
                builder.AppendLine(CultureInfo.CurrentCulture, $"[{TypeNameHelper.GetTypeDisplayName(entry.Value)}] {entry.Key}:'{entry.Value}'");
            }

            builder.AppendLine(SectionSeparator);
        }

        builder.Append(exception);
        return builder;
    }
}