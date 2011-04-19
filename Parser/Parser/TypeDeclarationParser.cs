using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Core;
using Purity.Compiler.Types;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Functors;
using Purity.Compiler.Modules;
using Purity.Compiler.TypeDeclarations;

namespace Purity.Compiler.Parser
{
    public static class TypeDeclarationParser
    {
        static Parser<string, string> ParseTypeParameter =
            from ident in Parsers.Identifier
            from indicator in Parsers.Match(Constants.TypeParameterIndicator)
            select ident;

        static Parser<string, Named<ITypeDeclaration>> ParseBox =
            from decl in Parsers.Match(Constants.TypeKeyword)
            from ws1 in Parsers.Whitespace
            from openingBracket in Parsers.Match(Constants.OpenTypeDeclaration)
            from ident in Parsers.Identifier
            from typeParameters in
                (
                    from ws in Parsers.WSChar.Rep1()
                    from typeParameter in ParseTypeParameter
                    select typeParameter
                ).Rep()
            from sc1 in Parsers.Whitespace.Then(Parsers.Match(Constants.TypeDeclarationSeparator)).Then(Parsers.Whitespace)
            from boxFunctionName in Parsers.Identifier.Or(Parsers.Return<string, string>(default(string)))
            from sc2 in Parsers.Whitespace.Then(Parsers.Match(Constants.TypeDeclarationSeparator)).Then(Parsers.Whitespace)
            from unboxFunctionName in Parsers.Identifier.Or(Parsers.Return<string, string>(default(string)))
            from ws2 in Parsers.Whitespace
            from closingBracket in Parsers.Match(Constants.CloseTypeDeclaration)
            from ws3 in Parsers.Whitespace
            from eq in Parsers.Match(Constants.EqualsSymbol)
            from ws4 in Parsers.Whitespace
            from type in TypeParser.ParseType
            select new Named<ITypeDeclaration>(ident,
                new BoxedTypeDeclaration(type, typeParameters.ToArray(), boxFunctionName, unboxFunctionName));

        static Parser<string, Named<ITypeDeclaration>> ParseLFix =
            from decl in Parsers.Match(Constants.TypeKeyword)
            from ws1 in Parsers.Whitespace
            from openingBracket in Parsers.Match(Constants.OpenTypeDeclaration)
            from ident in Parsers.Identifier
            from typeParameters in
                (
                    from ws in Parsers.WSChar.Rep1()
                    from typeParameter in ParseTypeParameter
                    select typeParameter
                ).Rep()
            from sc1 in Parsers.Whitespace.Then(Parsers.Match(Constants.TypeDeclarationSeparator)).Then(Parsers.Whitespace)
            from boxFunctionName in Parsers.Identifier.Or(Parsers.Return<string, string>(default(string)))
            from sc2 in Parsers.Whitespace.Then(Parsers.Match(Constants.TypeDeclarationSeparator)).Then(Parsers.Whitespace)
            from unboxFunctionName in Parsers.Identifier.Or(Parsers.Return<string, string>(default(string)))
            from sc3 in Parsers.Whitespace.Then(Parsers.Match(Constants.TypeDeclarationSeparator)).Then(Parsers.Whitespace)
            from cataFunctionName in Parsers.Identifier.Or(Parsers.Return<string, string>(default(string)))
            from ws2 in Parsers.Whitespace
            from closingBracket in Parsers.Match(Constants.CloseTypeDeclaration)
            from ws3 in Parsers.Whitespace
            from eq in Parsers.Match(Constants.EqualsSymbol)
            from ws4 in Parsers.Whitespace
            from lfix in Parsers.Match(Constants.Lfix)
            from ws5 in Parsers.WSChar.Rep1()
            from functor in FunctorParser.ParseFunctor
            select new Named<ITypeDeclaration>(ident,
                new LFixTypeDeclaration(functor, typeParameters.ToArray(), boxFunctionName, unboxFunctionName, cataFunctionName));

        static Parser<string, Named<ITypeDeclaration>> ParseGFix =
            from decl in Parsers.Match(Constants.TypeKeyword)
            from ws1 in Parsers.Whitespace
            from openingBracket in Parsers.Match(Constants.OpenTypeDeclaration)
            from ident in Parsers.Identifier
            from typeParameters in
                (
                    from ws in Parsers.WSChar.Rep1()
                    from typeParameter in ParseTypeParameter
                    select typeParameter
                ).Rep()
            from sc1 in Parsers.Whitespace.Then(Parsers.Match(Constants.TypeDeclarationSeparator)).Then(Parsers.Whitespace)
            from boxFunctionName in Parsers.Identifier.Or(Parsers.Return<string, string>(default(string)))
            from sc2 in Parsers.Whitespace.Then(Parsers.Match(Constants.TypeDeclarationSeparator)).Then(Parsers.Whitespace)
            from unboxFunctionName in Parsers.Identifier.Or(Parsers.Return<string, string>(default(string)))
            from sc3 in Parsers.Whitespace.Then(Parsers.Match(Constants.TypeDeclarationSeparator)).Then(Parsers.Whitespace)
            from anaFunctionName in Parsers.Identifier.Or(Parsers.Return<string, string>(default(string)))
            from ws2 in Parsers.Whitespace
            from closingBracket in Parsers.Match(Constants.CloseTypeDeclaration)
            from ws3 in Parsers.Whitespace
            from eq in Parsers.Match(Constants.EqualsSymbol)
            from ws4 in Parsers.Whitespace
            from lfix in Parsers.Match(Constants.Gfix)
            from ws5 in Parsers.WSChar.Rep1()
            from functor in FunctorParser.ParseFunctor
            select new Named<ITypeDeclaration>(ident,
                new GFixTypeDeclaration(functor, typeParameters.ToArray(), boxFunctionName, unboxFunctionName, anaFunctionName));

        public static Parser<string, Named<ITypeDeclaration>> ParseTypeDeclaration =
            ParseBox
            .Or(ParseLFix)
            .Or(ParseGFix);
    }
}
