//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Win32.Foundation;

// ReSharper disable InconsistentNaming
internal readonly partial struct PCSTR
{
#pragma warning disable CS0649
    public readonly unsafe byte* Value;
#pragma warning restore CS0649

    public static unsafe implicit operator PCSTR(byte* value)
    {
        return *(PCSTR*)&value;
    }

    public static unsafe implicit operator byte*(PCSTR value)
    {
        return *(byte**)&value;
    }
}

#if DEBUG
[global::System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
internal readonly partial struct PCSTR
{
    public unsafe string DebuggerDisplay
    {
        get => global::System.Text.Encoding.UTF8.GetString(global::System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpanFromNullTerminated(Value));
    }
}
#endif