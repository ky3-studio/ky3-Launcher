//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;
using kyxsan.Win32;
using System.IO;

namespace kyxsan.Core.IO;

internal static class ValueFileExtension
{
    extension(ValueFile file)
    {
        public async ValueTask<ValueResult<bool, T?>> DeserializeFromJsonNoThrowAsync<T>(JsonSerializerOptions options)
            where T : class
        {
            try
            {
                using (FileStream stream = File.OpenRead(file))
                {
                    T? t = await JsonSerializer.DeserializeAsync<T>(stream, options).ConfigureAwait(false);
                    return new(true, t);
                }
            }
            catch (Exception ex)
            {
                kyxsanNative.Instance.ShowErrorMessage(ex.Message, ExceptionFormat.Format(ex));
                return new(false, null);
            }
        }

        public async ValueTask<bool> SerializeToJsonNoThrowAsync<T>(T obj, JsonSerializerOptions options)
        {
            try
            {
                using (FileStream stream = File.Create(file))
                {
                    await JsonSerializer.SerializeAsync(stream, obj, options).ConfigureAwait(false);
                }

                return true;
            }
            catch (Exception ex)
            {
                kyxsanNative.Instance.ShowErrorMessage(ex.Message, ExceptionFormat.Format(ex));
                return false;
            }
        }
    }
}