// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Launcher.SourceGeneration.Primitive.SyntaxKeywords;

namespace Launcher.SourceGeneration.Primitive;

internal static partial class FastSyntaxFactory
{
    public static NullableTypeSyntax NullableObjectType { get; } = SyntaxFactory.NullableType(SyntaxFactory.PredefinedType(ObjectKeyword));

    public static NullableTypeSyntax NullableStringType { get; } = SyntaxFactory.NullableType(SyntaxFactory.PredefinedType(StringKeyword));

    public static PredefinedTypeSyntax BoolType { get; } = SyntaxFactory.PredefinedType(BoolKeyword);

    public static PredefinedTypeSyntax IntType { get; } = SyntaxFactory.PredefinedType(IntKeyword);

    public static PredefinedTypeSyntax ObjectType { get; } = SyntaxFactory.PredefinedType(ObjectKeyword);

    public static PredefinedTypeSyntax StringType { get; } = SyntaxFactory.PredefinedType(StringKeyword);

    public static PredefinedTypeSyntax UIntType { get; } = SyntaxFactory.PredefinedType(UIntKeyword);

    public static PredefinedTypeSyntax VoidType { get; } = SyntaxFactory.PredefinedType(VoidKeyword);
}