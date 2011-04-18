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
        static Parser<string, string> ParseTypeParameter =
            from ident in Parsers.Identifier
            from indicator in Parsers.Match(Constants.TypeParameterIndicator)
            select ident;

        static Parser<string, Named<FunctorDeclaration>> ParseNamedFunctor =
            from decl in Parsers.Match(Constants.FunctorKeyword)
            from ws1 in Parsers.WSChar.Rep1()
            from ident in Parsers.Identifier
            from typeParameters in
                (
                    from ws in Parsers.WSChar.Rep1()
                    from typeParameter in ParseTypeParameter
                    select typeParameter
                ).Rep()
            from ws2 in Parsers.Whitespace
            from eq in Parsers.Match(Constants.EqualsSymbol)
            from ws3 in Parsers.Whitespace
            from functor in FunctorParser.ParseFunctor
            select new Named<FunctorDeclaration>(ident,
                new FunctorDeclaration(functor, typeParameters.ToArray()));

        static Parser<string, Named<DataDeclaration>> ParseNamedUntypedData =
            from decl in Parsers.Match(Constants.DataKeyword)
            from ws1 in Parsers.WSChar.Rep1()
            from ident in Parsers.Identifier
            from ws2 in Parsers.Whitespace
            from rest in
                (from intro in Parsers.Match(Constants.DataTypeIntroduction)
                 from ws3 in Parsers.Whitespace
                 from type in TypeParser.ParseType
                 from ws4 in Parsers.Whitespace
                 from eq in Parsers.Match(Constants.EqualsSymbol)
                 from ws5 in Parsers.Whitespace
                 from data in DataParser.ParseData
                 select new Named<DataDeclaration>(ident, new DataDeclaration(type, data)))
                 .Or(from eq in Parsers.Match(Constants.EqualsSymbol)
                     from ws3 in Parsers.Whitespace
                     from data in DataParser.ParseData
                     select new Named<DataDeclaration>(ident, new DataDeclaration(data)))
            select rest;

        static Parser<string, ProgramElement> ParseProgramElement =
            (from functor in ParseNamedFunctor
             select new ProgramElement(functor)).Or(
             from type in TypeDeclarationParser.ParseTypeDeclaration
             select new ProgramElement(type)).Or(
             from data in ParseNamedUntypedData
             select new ProgramElement(data));

        static Parser<string, IEnumerable<ProgramElement>> ParseManyProgramElements =
            from p in ParseProgramElement
            from ps in
                (from ws in Parsers.WSChar.Rep1()
                 from parts in ParseManyProgramElements
                 select parts).Or(Parsers.EOF.Select(s => Enumerable.Empty<ProgramElement>()))
            select new[] { p }.Concat(ps);

        public static Parser<string, Module> ParseModule =
            ParseManyProgramElements.Select(es => new Module(es));
    }
}
