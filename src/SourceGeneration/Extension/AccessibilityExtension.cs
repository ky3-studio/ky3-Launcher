// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static kyxsan.SourceGeneration.Primitive.FastSyntaxFactory;

namespace kyxsan.SourceGeneration.Extension;

internal static class AccessibilityExtension
{
    public static SyntaxTokenList ToSyntaxTokenList(this Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.NotApplicable => TokenList(),
            Accessibility.Private => PrivateTokenList,
            Accessibility.ProtectedAndInternal => PrivateProtectedTokenList,
            Accessibility.Protected => ProtectedTokenList,
            Accessibility.Internal => InternalTokenList,
            Accessibility.ProtectedOrInternal => ProtectedInternalTokenList,
            Accessibility.Public => PublicTokenList,
            _ => TokenList()
        };
    }

    public static SyntaxTokenList ToSyntaxTokenList(this Accessibility accessibility, SyntaxToken additionalToken)
    {
        return ToSyntaxTokenList(accessibility).Add(additionalToken);
    }
}