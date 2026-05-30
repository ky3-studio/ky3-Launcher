//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace kyxsan.Core.LifeCycle.InterProcess;

// Layout:
// 0         1      2         3             4    Bytes
// ┌─────────┬──────┬─────────┬─────────────┐
// │ Version │ Type │ Command │ ContentType │
// ├─────────┴──────┴─────────┴─────────────┤ 4  Bytes
// │             ContentLength              │
// ├────────────────────────────────────────┤ 8  Bytes
// │                                        │
// │─────────────── Checksum ───────────────│
// │                                        │
// └────────────────────────────────────────┘ 16 Bytes
// Any content will be placed after the header.
[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct PipePacketHeader
{
    public byte Version;
    public PipePacketType Type;
    public PipePacketCommand Command;
    public PipePacketContentType ContentType;
    public int ContentLength;
    public ulong Checksum;
}