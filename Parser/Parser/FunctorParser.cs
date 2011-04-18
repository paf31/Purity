using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Core;
using Purity.Compiler.Functors;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Parser
{
    public static class FunctorParser
    {
        static Parser<string, IFunctor> ParseFunctorSynonym =
            from ident in Parsers.Identifier
            from typeParameters in
                (
                    from ws in Parsers.WSChar.Rep1()
                    from typeParameter in TypeParser.ParseType
                    select typeParameter
                ).Rep()
            select(IFunctor)new FunctorSynonym(ident, typeParameters.ToArray());

        static Parser<string, IFunctor> ParseConstant =
            from open in Parsers.Match(Constants.Const)
            from ws in Parsers.WSChar.Rep1()
            from type in TypeParser.ParseType
            select (IFunctor)new ConstantFunctor(type);

        static Parser<string, IFunctor> ParseIdentity =
            Parsers.Match(Constants.Id).Select(s => (IFunctor)new IdentityFunctor());

        static Parser<string, IFunctor> ParseBrackettedFunctor =
            from open in Parsers.Match(Constants.OpeningBracket)
            from ws1 in Parsers.Whitespace
            from functor in ParseFunctor
            from ws2 in Parsers.Whitespace
            from close in Parsers.Match(Constants.ClosingBracket)
            select functor;

        static Parser<string, IFunctor> ParseAtom =
            ParseIdentity
            .Or(ParseConstant)
            .Or(ParseFunctorSynonym)
            .Or(ParseBrackettedFunctor);

        static Parser<string, IFunctor> ParseProductOfAtoms =
            from first in ParseAtom
            from opt in
                (from ws1 in Parsers.Whitespace
                 from op in Parsers.Match(Constants.FunctorOperatorProduct)
                 from ws2 in Parsers.Whitespace
                 from product in ParseProductOfAtoms
                 select (IFunctor)new ProductFunctor(first, product))
                 .Or(Parsers.Return<string, IFunctor>(first))
            select opt;

        static Parser<string, IFunctor> ParseSumOfProductsOfAtoms =
            from first in ParseProductOfAtoms
            from opt in
                (from ws1 in Parsers.Whitespace
                 from op in Parsers.Match(Constants.FunctorOperatorSum)
                 from ws2 in Parsers.Whitespace
                 from sum in ParseSumOfProductsOfAtoms
                 select (IFunctor)new SumFunctor(first, sum))
                .Or(Parsers.Return<string, IFunctor>(first))
            select opt;

        public static Parser<string, IFunctor> ParseFunctor =
            (from type in TypeParser.ParseType
             from ws1 in Parsers.Whitespace
             from op in Parsers.Match(Constants.FunctorOperatorArrow)
             from ws2 in Parsers.Whitespace
             from functor in ParseFunctor
             select (IFunctor)new ArrowFunctor(type, functor)).Or(ParseSumOfProductsOfAtoms);
    }
}
