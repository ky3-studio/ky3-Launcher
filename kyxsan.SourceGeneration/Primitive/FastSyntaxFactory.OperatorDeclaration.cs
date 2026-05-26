// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace kyxsan.SourceGeneration.Primitive;

internal static partial class FastSyntaxFactory
{
    public static OperatorDeclarationSyntax EqualsEqualsOperatorDeclaration(TypeSyntax returnType)
    {
        return SyntaxFactory.OperatorDeclaration(returnType, SyntaxFactory.Token(SyntaxKind.EqualsEqualsToken));
    }

    public static OperatorDeclarationSyntax ExclamationEqualsOperatorDeclaration(TypeSyntax returnType)
    {
        return SyntaxFactory.OperatorDeclaration(returnType, SyntaxFactory.Token(SyntaxKind.ExclamationEqualsToken));
    }

    public static OperatorDeclarationSyntax MinusOperatorDeclaration(TypeSyntax returnType)
    {
        return SyntaxFactory.OperatorDeclaration(returnType, SyntaxFactory.Token(SyntaxKind.MinusToken));
    }

    public static OperatorDeclarationSyntax MinusMinusOperatorDeclaration(TypeSyntax returnType)
    {
        return SyntaxFactory.OperatorDeclaration(returnType, SyntaxFactory.Token(SyntaxKind.MinusMinusToken));
    }

    public static OperatorDeclarationSyntax PlusOperatorDeclaration(TypeSyntax returnType)
    {
        return SyntaxFactory.OperatorDeclaration(returnType, SyntaxFactory.Token(SyntaxKind.PlusToken));
    }

    public static OperatorDeclarationSyntax PlusPlusOperatorDeclaration(TypeSyntax returnType)
    {
        return SyntaxFactory.OperatorDeclaration(returnType, SyntaxFactory.Token(SyntaxKind.PlusPlusToken));
    }
}