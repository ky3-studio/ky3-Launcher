//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Net.Http.Headers;

namespace kyxsan.Web;

internal static class HttpHeadersExtension
{
    extension(HttpHeaders headers)
    {
        public void AddWithUnknownValueCount(string name, IEnumerable<string?>? values)
        {
            ArgumentNullException.ThrowIfNull(name);

            values ??= [];
            values = values.Where(v => v is not null);

            // ReSharper disable once PossibleMultipleEnumeration
            if (values.Any())
            {
                // ReSharper disable once PossibleMultipleEnumeration
                headers.Add(name, values);
            }
            else
            {
                headers.Add(name, string.Empty);
            }
        }

        public IEnumerable<string>? GetValuesOrDefault(string name)
        {
            _ = headers.TryGetValues(name, out IEnumerable<string>? values);
            return values;
        }

        public void Remove(IEnumerable<string?>? names)
        {
            if (names is null)
            {
                return;
            }

            foreach (string? name in names)
            {
                if (name is not null)
                {
                    headers.Remove(name);
                }
            }
        }
    }
}