//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Core.Setting;

internal static class UnsafeLocalSetting
{
    public static unsafe TEnum Get<TEnum>(string key, TEnum defaultValue = default!)
        where TEnum : unmanaged, Enum
    {
        switch (Type.GetTypeCode(typeof(TEnum)))
        {
            case TypeCode.Byte:
                {
                    byte result = LocalSetting.Get(key, *(byte*)&defaultValue);
                    return *(TEnum*)&result;
                }

            case TypeCode.Int16:
                {
                    short result = LocalSetting.Get(key, *(short*)&defaultValue);
                    return *(TEnum*)&result;
                }

            case TypeCode.UInt16:
                {
                    ushort result = LocalSetting.Get(key, *(ushort*)&defaultValue);
                    return *(TEnum*)&result;
                }

            case TypeCode.Int32:
                {
                    int result = LocalSetting.Get(key, *(int*)&defaultValue);
                    return *(TEnum*)&result;
                }

            case TypeCode.UInt32:
                {
                    uint result = LocalSetting.Get(key, *(uint*)&defaultValue);
                    return *(TEnum*)&result;
                }

            case TypeCode.Int64:
                {
                    long result = LocalSetting.Get(key, *(long*)&defaultValue);
                    return *(TEnum*)&result;
                }

            default:
                // sbyte not supported
                throw new InvalidCastException();
        }
    }

    public static unsafe void Set<TEnum>(string key, TEnum value)
        where TEnum : unmanaged, Enum
    {
        switch (Type.GetTypeCode(typeof(TEnum)))
        {
            case TypeCode.Byte:
                LocalSetting.Set(key, *(byte*)&value);
                break;
            case TypeCode.Int16:
                LocalSetting.Set(key, *(short*)&value);
                break;
            case TypeCode.UInt16:
                LocalSetting.Set(key, *(ushort*)&value);
                break;
            case TypeCode.Int32:
                LocalSetting.Set(key, *(int*)&value);
                break;
            case TypeCode.UInt32:
                LocalSetting.Set(key, *(uint*)&value);
                break;
            case TypeCode.Int64:
                LocalSetting.Set(key, *(long*)&value);
                break;
            case TypeCode.UInt64:
                LocalSetting.Set(key, *(ulong*)&value);
                break;
            default:
                throw new InvalidCastException();
        }
    }
}