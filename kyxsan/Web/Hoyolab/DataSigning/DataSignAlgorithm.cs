//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.IO.Hashing;
using System.Security.Cryptography;

namespace kyxsan.Web.Hoyolab.DataSigning;

internal static class DataSignAlgorithm
{
    public static string GetDataSign(DataSignOptions options)
    {
        long t = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string r = options.RandomString;

        string dsContent = $"salt={options.Salt}&t={t}&r={options.RandomString}";

        // ds2 b & q process
        if (options.RequiresBodyAndQuery)
        {
            dsContent = $"{dsContent}&b={options.Body}&q={options.Query}";
        }

        string check = Hash.ToHexStringLower(HashAlgorithmName.MD5, dsContent);
        return $"{t},{r},{check}";
    }
}