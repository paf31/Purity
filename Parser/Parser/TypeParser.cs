using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Core;
using Purity.Compiler.Types;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Functors;

namespace Purity.Compiler.Parser
{
    public static class TypeParser
    {
        static Parser<string, IType> ParseTypeSynonymOrParameter =
            from ident in Parsers.Identifier
            from type in
                (
                    Parsers.Match(Constants.TypeParameterIndicator)
                    .Select(_ => (IType)new TypeParameter(ident))
                    .Or(from typeParameters in
                            (
                                from ws in Parsers.WSChar.Rep1()
                                from typeParameter in ParseAtom
                                select typeParameter
                            ).Rep()
                        select (IType)new TypeSynonym(ident, typeParameters.ToArray())))
            select type;

        static Parser<string, IType> ParseBrackettedType =
            from open in Parsers.Match(Constants.OpeningBracket)
            from ws1 in Parsers.Whitespace
            from type in ParseType
            from ws2 in Parsers.Whitespace
            from close in Parsers.Match(Constants.ClosingBracket)
            select type;

        static Parser<string, IType> ParseAtom =
            ParseTypeSynonymOrParameter
            .Or(ParseBrackettedType);

        static Parser<string, IType> ParseProductOfAtoms =
            from first in ParseAtom
            from opt in
                (from ws1 in Parsers.Whitespace
                 from op in Parsers.Match(Constants.TypeOperatorProduct)
                 from ws2 in Parsers.Whitespace
                 from product in ParseProductOfAtoms
                 select (IType)new ProductType(first, product))
                 .Or(Parsers.Return<string, IType>(first))
            select opt;

        static Parser<string, IType> ParseSumOfProductsOfAtoms =
            from first in ParseProductOfAtoms
            from opt in
                (from ws1 in Parsers.Whitespace
                 from op in Parsers.Match(Constants.TypeOperatorSum)
                 from ws2 in Parsers.Whitespace
                 from sum in ParseSumOfProductsOfAtoms
                 select (IType)new SumType(first, sum))
                .Or(Parsers.Return<string, IType>(first))
            select opt;

        public static Parser<string, IType> ParseType =
            from first in ParseSumOfProductsOfAtoms
            from opt in
                (from ws1 in Parsers.Whitespace
                 from op in Parsers.Match(Constants.TypeOperatorArrow)
                 from ws2 in Parsers.Whitespace
                 from arrow in ParseType
                 select (IType)new ArrowType(first, arrow))
                .Or(Parsers.Return<string, IType>(first))
            select opt;
    }
}
