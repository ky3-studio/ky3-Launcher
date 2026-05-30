//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___          __   __ _    _____
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \  __  __ \ \ / // \  | ____|
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | | \ \/ /  \ V // _ \ |  _|
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |  >  <    | |/ ___ \| |___
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/  /_/\_\   |_/_/   \_\_____|
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Google.Protobuf;
using kyxsan.Core.Protobuf;
using System.Buffers;
using System.Runtime.InteropServices;

namespace kyxsan.Core.LifeCycle.InterProcess.Yae;

internal sealed partial class YaeData : IDisposable
{
    private readonly IMemoryOwner<byte> owner;
    private readonly int contentLength;

    public YaeData(YaeCommandKind kind, IMemoryOwner<byte> owner, int contentLength)
    {
        Kind = kind;
        this.owner = owner;
        this.contentLength = contentLength;
    }

    ~YaeData()
    {
        Dispose();
    }

    public static YaeData SessionEnd { get => new(YaeCommandKind.SessionEnd, IMemoryOwner<byte>.Empty, 0); }

    public YaeCommandKind Kind { get; }

    public ByteString Bytes { get => ByteStringMarshal.Create(owner.Memory[..contentLength]); }

    public ref readonly YaePropertyTypeValue PropertyTypeValue
    {
        get => ref MemoryMarshal.AsRef<YaePropertyTypeValue>(owner.Memory.Span[..contentLength]);
    }

    public void Dispose()
    {
        owner.Dispose();
        GC.SuppressFinalize(this);
    }
}