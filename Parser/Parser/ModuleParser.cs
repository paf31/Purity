using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Functors;
using Purity.Compiler.Modules;
using Purity.Compiler.Types;
using Purity.Compiler.Data;
using Purity.Compiler.Interfaces;
using Purity.Compiler.TypedExpressions;

namespace Purity.Compiler.Parser
{
    public static class ModuleParser
    {
        static Parser<string, Named<IFunctor>> ParseNamedFunctor = from decl in Parsers.Match(Constants.FunctorKeyword)
                                                                   from ws1 in Parsers.WSChar.Rep1()
                                                                   from ident in Parsers.Identifier
                                                                   from ws2 in Parsers.Whitespace
                                                                   from eq in Parsers.Match(Constants.EqualsSymbol)
                                                                   from ws3 in Parsers.Whitespace
                                                                   from functor in FunctorParser.ParseFunctor
                                                                   select new Named<IFunctor>(ident, functor);

        static Parser<string, Named<IType>> ParseNamedType = from decl in Parsers.Match(Constants.TypeKeyword)
                                                             from ws1 in Parsers.WSChar.Rep1()
                                                             from ident in Parsers.Identifier
                                                             from ws2 in Parsers.Whitespace
                                                             from eq in Parsers.Match(Constants.EqualsSymbol)
                                                             from ws3 in Parsers.Whitespace
                                                             from type in TypeParser.ParseType
                                                             select new Named<IType>(ident, type);

        static Parser<string, Named<DataDeclaration>> ParseNamedUntypedData = from decl in Parsers.Match(Constants.DataKeyword)
                                                                              from ws1 in Parsers.WSChar.Rep1()
                                                                              from ident in Parsers.Identifier
                                                                              from ws2 in Parsers.Whitespace
                                                                              from intro in Parsers.Match(Constants.DataTypeIntroduction)
                                                                              from ws3 in Parsers.Whitespace
                                                                              from type in TypeParser.ParseType
                                                                              from ws4 in Parsers.Whitespace
                                                                              from eq in Parsers.Match(Constants.EqualsSymbol)
                                                                              from ws5 in Parsers.Whitespace
                                                                              from data in DataParser.ParseData
                                                                              select new Named<DataDeclaration>(ident, new DataDeclaration(type, data));

        static Parser<string, ProgramElement> ParseProgramElement =
            (from functor in ParseNamedFunctor
             select new ProgramElement(functor)).Or(
             from type in ParseNamedType
             select new ProgramElement(type)).Or(
             from data in ParseNamedUntypedData
             select new ProgramElement(data));

        public static Parser<string, Module> ParseModule = 
            from p in ParseProgramElement
            from ps in
                (from ws in Parsers.WSChar.Rep1()
                 from part in ParseProgramElement
                 select part).Rep1()
            select new Module(new[] { p }.Concat(ps).ToArray());

    }
}
