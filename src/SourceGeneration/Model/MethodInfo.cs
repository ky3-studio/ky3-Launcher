// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Launcher.SourceGeneration.Extension;
using System.Collections.Immutable;

namespace Launcher.SourceGeneration.Model;

internal sealed record MethodInfo
{
    public required string Name { get; init; }

    public required string FullyQualifiedReturnTypeName { get; init; }

    public required string FullyQualifiedReturnTypeMetadataName { get; init; }

    public required EquatableArray<ParameterInfo> Parameters { get; init; }

    public required bool IsStatic { get; init; }

    public static MethodInfo Create(IMethodSymbol methodSymbol)
    {
        return new()
        {
            Name = methodSymbol.Name,
            FullyQualifiedReturnTypeName = methodSymbol.ReturnType.GetFullyQualifiedNameWithNullabilityAnnotations(),
            FullyQualifiedReturnTypeMetadataName = methodSymbol.ReturnType.GetFullyQualifiedMetadataName(),
            Parameters = ImmutableArray.CreateRange(methodSymbol.Parameters, ParameterInfo.Create),
            IsStatic = methodSymbol.IsStatic,
        };
    }
}