//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace kyxsan.Service.Game.Account;

internal static unsafe class WindowsHDRControl
{
    [DllImport("user32.dll")]
    private static extern int GetDisplayConfigBufferSizes(
        uint flags, uint* numPathArrayElements, uint* numModeInfoArrayElements);

    [DllImport("user32.dll")]
    private static extern int QueryDisplayConfig(
        uint flags, uint* numPathArrayElements, DISPLAYCONFIG_PATH_INFO* pathArray,
        uint* numModeInfoArrayElements, DISPLAYCONFIG_MODE_INFO* modeInfoArray, nint currentTopologyId);

    [DllImport("user32.dll")]
    private static extern int DisplayConfigGetDeviceInfo(DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO* requestPacket);

    [DllImport("user32.dll")]
    private static extern int DisplayConfigSetDeviceInfo(DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE* setPacket);

    private const uint QDC_ONLY_ACTIVE_PATHS = 0x00000002;
    private const int ERROR_SUCCESS = 0;
    private const int DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO = 9;
    private const int DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE = 10;

    [StructLayout(LayoutKind.Sequential)]
    private struct LUID
    {
        public uint LowPart;
        public int HighPart;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct DISPLAYCONFIG_PATH_SOURCE_INFO
    {
        public LUID adapterId;
        public uint id;
        public uint modeInfoIdx;
        public uint statusFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct DISPLAYCONFIG_RATIONAL
    {
        public uint Numerator;
        public uint Denominator;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct DISPLAYCONFIG_PATH_TARGET_INFO
    {
        public LUID adapterId;
        public uint id;
        public uint modeInfoIdx;
        public uint outputTechnology;
        public uint rotation;
        public uint scaling;
        public DISPLAYCONFIG_RATIONAL refreshRate;
        public uint scanLineOrdering;
        public int targetAvailable; // Windows BOOL = 4 bytes
        public uint statusFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct DISPLAYCONFIG_PATH_INFO
    {
        public DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo;
        public DISPLAYCONFIG_PATH_TARGET_INFO targetInfo;
        public uint flags;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct DISPLAYCONFIG_MODE_INFO
    {
        [FieldOffset(0)] public uint infoType;
        [FieldOffset(4)] public uint id;
        [FieldOffset(8)] public LUID adapterId;
        [FieldOffset(16)] private ulong padding1;
        [FieldOffset(24)] private ulong padding2;
        [FieldOffset(32)] private ulong padding3;
        [FieldOffset(40)] private ulong padding4;
        [FieldOffset(48)] private ulong padding5;
        [FieldOffset(56)] private ulong padding6;
        [FieldOffset(64)] private ulong padding7;
        [FieldOffset(72)] private ulong padding8;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct DISPLAYCONFIG_DEVICE_INFO_HEADER
    {
        public int type;
        public uint size;
        public LUID adapterId;
        public uint id;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO
    {
        public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
        public uint value;
        public uint colorEncoding;
        public uint bitsPerColorChannel;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE
    {
        public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
        public uint enableAdvancedColor;
    }

    private static bool QueryPaths(
        out DISPLAYCONFIG_PATH_INFO* paths, out uint pathCount,
        out DISPLAYCONFIG_MODE_INFO* modes, out uint modeCount)
    {
        paths = null;
        modes = null;
        pathCount = 0;
        modeCount = 0;

        uint pc = 0, mc = 0;
        int result = GetDisplayConfigBufferSizes(QDC_ONLY_ACTIVE_PATHS, &pc, &mc);
        if (result != ERROR_SUCCESS || pc == 0)
            return false;

        DISPLAYCONFIG_PATH_INFO* pBuf = (DISPLAYCONFIG_PATH_INFO*)NativeMemory.AllocZeroed(
            pc, (nuint)Unsafe.SizeOf<DISPLAYCONFIG_PATH_INFO>());
        DISPLAYCONFIG_MODE_INFO* mBuf = (DISPLAYCONFIG_MODE_INFO*)NativeMemory.AllocZeroed(
            mc, (nuint)Unsafe.SizeOf<DISPLAYCONFIG_MODE_INFO>());

        result = QueryDisplayConfig(QDC_ONLY_ACTIVE_PATHS, &pc, pBuf, &mc, mBuf, 0);
        if (result != ERROR_SUCCESS)
        {
            NativeMemory.Free(pBuf);
            NativeMemory.Free(mBuf);
            return false;
        }

        paths = pBuf;
        modes = mBuf;
        pathCount = pc;
        modeCount = mc;
        return true;
    }

    public static string DiagnoseHDR()
    {
        try
        {
            uint pc = 0, mc = 0;
            int result = GetDisplayConfigBufferSizes(QDC_ONLY_ACTIVE_PATHS, &pc, &mc);
            if (result != ERROR_SUCCESS)
                return $"GetDisplayConfigBufferSizes failed: {result}";
            if (pc == 0)
                return $"pathCount=0, modeCount={mc}";

            if (!QueryPaths(out DISPLAYCONFIG_PATH_INFO* paths, out uint pathCount, out DISPLAYCONFIG_MODE_INFO* modes, out uint modeCount))
                return $"QueryDisplayConfig failed, pc={pc} mc={mc}";

            try
            {
                string info = $"paths={pathCount} modes={modeCount}\n";
                for (uint i = 0; i < pathCount; i++)
                {
                    ref DISPLAYCONFIG_PATH_INFO path = ref paths[i];
                    DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO colorInfo = new()
                    {
                        header = new DISPLAYCONFIG_DEVICE_INFO_HEADER
                        {
                            type = DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO,
                            size = (uint)Unsafe.SizeOf<DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO>(),
                            adapterId = path.targetInfo.adapterId,
                            id = path.targetInfo.id,
                        },
                    };

                    int devResult = DisplayConfigGetDeviceInfo(&colorInfo);
                    bool supported = devResult == ERROR_SUCCESS && (colorInfo.value & 0x1) != 0;
                    bool enabled = devResult == ERROR_SUCCESS && (colorInfo.value & 0x2) != 0;
                    info += $"[{i}] ret={devResult} val=0x{colorInfo.value:X} sup={supported} en={enabled} id={path.targetInfo.id}\n";
                }

                return info;
            }
            finally
            {
                NativeMemory.Free(paths);
                NativeMemory.Free(modes);
            }
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}\n{ex.StackTrace}";
        }
    }

    public static bool IsHDRSupported()
    {
        try
        {
            if (!QueryPaths(out DISPLAYCONFIG_PATH_INFO* paths, out uint pathCount, out DISPLAYCONFIG_MODE_INFO* modes, out _))
                return false;

            try
            {
                for (uint i = 0; i < pathCount; i++)
                {
                    ref DISPLAYCONFIG_PATH_INFO path = ref paths[i];
                    DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO colorInfo = new()
                    {
                        header = new DISPLAYCONFIG_DEVICE_INFO_HEADER
                        {
                            type = DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO,
                            size = (uint)Unsafe.SizeOf<DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO>(),
                            adapterId = path.targetInfo.adapterId,
                            id = path.targetInfo.id,
                        },
                    };

                    int result = DisplayConfigGetDeviceInfo(&colorInfo);
                    if (result == ERROR_SUCCESS && (colorInfo.value & 0x1) != 0)
                        return true;
                }

                return false;
            }
            finally
            {
                NativeMemory.Free(paths);
                NativeMemory.Free(modes);
            }
        }
        catch
        {
            return false;
        }
    }

    public static bool IsHDROn()
    {
        try
        {
            if (!QueryPaths(out DISPLAYCONFIG_PATH_INFO* paths, out uint pathCount, out DISPLAYCONFIG_MODE_INFO* modes, out _))
                return false;

            try
            {
                for (uint i = 0; i < pathCount; i++)
                {
                    ref DISPLAYCONFIG_PATH_INFO path = ref paths[i];
                    DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO colorInfo = new()
                    {
                        header = new DISPLAYCONFIG_DEVICE_INFO_HEADER
                        {
                            type = DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO,
                            size = (uint)Unsafe.SizeOf<DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO>(),
                            adapterId = path.targetInfo.adapterId,
                            id = path.targetInfo.id,
                        },
                    };

                    int result = DisplayConfigGetDeviceInfo(&colorInfo);
                    if (result == ERROR_SUCCESS)
                    {
                        bool supported = (colorInfo.value & 0x1) != 0;
                        bool enabled = (colorInfo.value & 0x2) != 0;
                        if (supported)
                            return enabled;
                    }
                }

                return false;
            }
            finally
            {
                NativeMemory.Free(paths);
                NativeMemory.Free(modes);
            }
        }
        catch
        {
            return false;
        }
    }

    public static bool SetHDR(bool enable)
    {
        try
        {
            if (!QueryPaths(out DISPLAYCONFIG_PATH_INFO* paths, out uint pathCount, out DISPLAYCONFIG_MODE_INFO* modes, out _))
                return false;

            try
            {
                bool anySuccess = false;
                for (uint i = 0; i < pathCount; i++)
                {
                    ref DISPLAYCONFIG_PATH_INFO path = ref paths[i];
                    DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO colorInfo = new()
                    {
                        header = new DISPLAYCONFIG_DEVICE_INFO_HEADER
                        {
                            type = DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO,
                            size = (uint)Unsafe.SizeOf<DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO>(),
                            adapterId = path.targetInfo.adapterId,
                            id = path.targetInfo.id,
                        },
                    };

                    int result = DisplayConfigGetDeviceInfo(&colorInfo);
                    if (result != ERROR_SUCCESS)
                        continue;

                    bool supported = (colorInfo.value & 0x1) != 0;
                    if (!supported)
                        continue;

                    DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE setState = new()
                    {
                        header = new DISPLAYCONFIG_DEVICE_INFO_HEADER
                        {
                            type = DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE,
                            size = (uint)Unsafe.SizeOf<DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE>(),
                            adapterId = path.targetInfo.adapterId,
                            id = path.targetInfo.id,
                        },
                        enableAdvancedColor = enable ? 1u : 0u,
                    };

                    result = DisplayConfigSetDeviceInfo(&setState);
                    if (result == ERROR_SUCCESS)
                        anySuccess = true;
                }

                return anySuccess;
            }
            finally
            {
                NativeMemory.Free(paths);
                NativeMemory.Free(modes);
            }
        }
        catch
        {
            return false;
        }
    }
}
