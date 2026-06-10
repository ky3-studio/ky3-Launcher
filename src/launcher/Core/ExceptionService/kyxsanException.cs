//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace kyxsan.Core.ExceptionService;

internal sealed class kyxsanException : Exception
{
    [StackTraceHidden]
    public kyxsanException(string message, Exception? innerException)
        : base($"{message}\n{innerException?.Message}", innerException)
    {
    }

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static kyxsanException Throw(string message, Exception? innerException = default)
    {
        throw new kyxsanException(message, innerException);
    }

    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowIf([DoesNotReturnIf(true)] bool condition, string message, Exception? innerException = default)
    {
        if (condition)
        {
            throw new kyxsanException(message, innerException);
        }
    }

    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowIfNot([DoesNotReturnIf(false)] bool condition, string message, Exception? innerException = default)
    {
        if (!condition)
        {
            throw new kyxsanException(message, innerException);
        }
    }

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static ArgumentException Argument(string message, string? paramName)
    {
        throw new ArgumentException(message, paramName);
    }

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static kyxsanException GachaStatisticsInvalidItemId(uint id, Exception? innerException = default)
    {
        throw new kyxsanException(SH.FormatServiceGachaStatisticsFactoryItemIdInvalid(id), innerException);
    }

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static InvalidCastException InvalidCast<TFrom, TTo>(string name, Exception? innerException = default)
    {
        string message = $"This instance of '{typeof(TFrom).FullName}' '{name}' doesn't implement '{typeof(TTo).FullName}'";
        throw new InvalidCastException(message, innerException);
    }

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static InvalidOperationException InvalidOperation(string message, Exception? innerException = default)
    {
        throw new InvalidOperationException(message, innerException);
    }

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static IOException IO(string message, Exception? innerException = default)
    {
        throw new IOException(message, innerException);
    }

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static NotSupportedException NotSupported(string? message = default, Exception? innerException = default)
    {
        throw new NotSupportedException(message, innerException);
    }

    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void NotSupportedIf([DoesNotReturnIf(true)] bool condition, string? message = default, Exception? innerException = default)
    {
        if (condition)
        {
            throw new NotSupportedException(message, innerException);
        }
    }

    [StackTraceHidden]
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static OperationCanceledException OperationCanceled(string message, Exception? innerException = default)
    {
        throw new OperationCanceledException(message, innerException);
    }

    public static UnauthorizedAccessException UnauthorizedAccess(string message, Exception? innerException = default)
    {
        throw new UnauthorizedAccessException(message, innerException);
    }
}