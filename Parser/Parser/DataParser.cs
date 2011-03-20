using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Data;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Parser
{
    public static class DataParser
    {
        static Parser<string, IData> ParseIn = Parsers.Match(Constants.In).Select(o => (IData)new In());

        static Parser<string, IData> ParseOut = Parsers.Match(Constants.Out).Select(o => (IData)new Out());

        static Parser<string, IData> ParseInl = Parsers.Match(Constants.Inl).Select(o => (IData)new Inl());

        static Parser<string, IData> ParseInr = Parsers.Match(Constants.Inr).Select(o => (IData)new Inr());

        static Parser<string, IData> ParseOutl = Parsers.Match(Constants.Outl).Select(o => (IData)new Outl());

        static Parser<string, IData> ParseOutr = Parsers.Match(Constants.Outr).Select(o => (IData)new Outr());

        static Parser<string, IData> ParseIdentity = Parsers.Match(Constants.Id).Select(o => (IData)new Identity());

        static Parser<string, IData> ParseBox = Parsers.Match(Constants.Box).Select(o => (IData)new Box());

        static Parser<string, IData> ParseUnbox = Parsers.Match(Constants.Unbox).Select(o => (IData)new Unbox());

        static Parser<string, IData> ParseDataSynonym = from ident in Parsers.Identifier
                                                        select (IData)new DataSynonym(ident);

        static Parser<string, IData> ParseAna = from open in Parsers.Match(Constants.AnaOpeningBrace)
                                                from ws1 in Parsers.Whitespace
                                                from coalgebra in ParseData
                                                from ws2 in Parsers.Whitespace
                                                from close in Parsers.Match(Constants.AnaClosingBrace)
                                                select (IData)new Application(new Ana(), coalgebra);

        static Parser<string, IData> ParseCata = from open in Parsers.Match(Constants.CataOpeningBrace)
                                                 from ws1 in Parsers.Whitespace
                                                 from algebra in ParseData
                                                 from ws2 in Parsers.Whitespace
                                                 from close in Parsers.Match(Constants.CataClosingBrace)
                                                 select (IData)new Application(new Cata(), algebra);

        static Parser<string, IData> ParseConst = from open in Parsers.Match(Constants.Const)
                                                  from ws in Parsers.Whitespace
                                                  from value in ParseData
                                                  select (IData)new Const(value);

        static Parser<string, IData> ParseCase = from open in Parsers.Match(Constants.CaseOpeningBrace)
                                                 from ws1 in Parsers.Whitespace
                                                 from left in ParseData
                                                 from ws2 in Parsers.Whitespace
                                                 from comma in Parsers.Match(Constants.CaseSeparator)
                                                 from ws3 in Parsers.Whitespace
                                                 from right in ParseData
                                                 from ws4 in Parsers.Whitespace
                                                 from close in Parsers.Match(Constants.CaseClosingBrace)
                                                 select (IData)new Case(left, right);

        static Parser<string, IData> ParseSplit = from open in Parsers.Match(Constants.SplitOpeningBrace)
                                                  from ws1 in Parsers.Whitespace
                                                  from left in ParseData
                                                  from ws2 in Parsers.Whitespace
                                                  from comma in Parsers.Match(Constants.SplitSeparator)
                                                  from ws3 in Parsers.Whitespace
                                                  from right in ParseData
                                                  from ws4 in Parsers.Whitespace
                                                  from close in Parsers.Match(Constants.SplitClosingBrace)
                                                  select (IData)new Split(left, right);

        static Parser<string, IData> ParseCurriedFunction = from curry in Parsers.Match(Constants.Curry)
                                                            from ws in Parsers.WSChar.Rep1()
                                                            from function in ParseData
                                                            select (IData)new Curried(function);

        static Parser<string, IData> ParseUncurriedFunction = from curry in Parsers.Match(Constants.Uncurry)
                                                              from ws in Parsers.WSChar.Rep1()
                                                              from function in ParseData
                                                              select (IData)new Uncurried(function);

        static Parser<string, IData> ParseBrackettedData = from open in Parsers.Match(Constants.OpeningBracket)
                                                           from ws1 in Parsers.Whitespace
                                                           from data in ParseData
                                                           from ws2 in Parsers.Whitespace
                                                           from close in Parsers.Match(Constants.ClosingBracket)
                                                           select data;

        static Parser<string, IData> ParseVariable = from introduction in Parsers.Match(Constants.VariableIntroduction)
                                                     from name in Parsers.Identifier
                                                     select (IData)new Variable(name);

        static Parser<string, IData> ParseAbstraction = from variable in Parsers.Identifier
                                                        from ws1 in Parsers.Whitespace
                                                        from lambdaOperator in Parsers.Match(Constants.LambdaOperator)
                                                        from ws2 in Parsers.Whitespace
                                                        from body in ParseData
                                                        select (IData)new Abstraction(variable, body);

        static Parser<string, IData> ParseAtom =
            ParseAna
            .Or(ParseBox)
            .Or(ParseUnbox)
            .Or(ParseCata)
            .Or(ParseCase)
            .Or(ParseSplit)
            .Or(ParseAbstraction)
            .Or(ParseVariable)
            .Or(ParseDataSynonym)
            .Or(ParseInl)
            .Or(ParseInr)
            .Or(ParseIn)
            .Or(ParseOutl)
            .Or(ParseOutr)
            .Or(ParseOut)
            .Or(ParseConst)
            .Or(ParseIdentity)
            .Or(ParseUncurriedFunction)
            .Or(ParseCurriedFunction)
            .Or(ParseBrackettedData);

        static Parser<string, IData> ParseCompositionOfAtoms =
           from left in ParseAtom
           from opt in
               (from ws1 in Parsers.Whitespace
                from dot in Parsers.Match(Constants.CompositionOperator)
                from ws2 in Parsers.Whitespace
                from right in ParseCompositionOfAtoms
                select (IData)new Composition(left, right))
                .Or(Parsers.Return<string, IData>(left))
           select opt;

        static Parser<string, IData[]> ParseManyParts =
            from first in ParseCompositionOfAtoms
            from rest in
                (from ws in Parsers.WSChar.Rep1()
                 from data in ParseManyParts
                 select data)
                 .Or(Parsers.Return<string, IData[]>(new IData[0]))
            select new[] { first }.Concat(rest).ToArray();

        public static Parser<string, IData> ParseData =
            from parts in ParseManyParts
            select parts.Aggregate((f, a) => new Application(f, a));
    }
}
