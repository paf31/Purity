using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Data;
using Purity.Compiler.TypedExpressions;
using Purity.Compiler.Modules;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler
{
    public static class Container
    {
        private static IDictionary<string, ITypeDeclaration> types =
            new Dictionary<string, ITypeDeclaration>();

        private static IDictionary<string, FunctorDeclaration> functors =
            new Dictionary<string, FunctorDeclaration>();

        private static IDictionary<string, DataDeclaration> data =
            new Dictionary<string, DataDeclaration>();

        public static ITypeDeclaration ResolveType(string identifier)
        {
            if (!types.ContainsKey(identifier))
            {
                throw new CompilerException(string.Format(ErrorMessages.UnableToResolveType, identifier));
            }

            return types[identifier];
        }

        public static FunctorDeclaration ResolveFunctor(string identifier)
        {
            if (!functors.ContainsKey(identifier))
            {
                throw new CompilerException(string.Format(ErrorMessages.UnableToResolveFunctor, identifier));
            }

            return functors[identifier];
        }

        public static DataDeclaration ResolveValue(string identifier)
        {
            if (!data.ContainsKey(identifier))
            {
                throw new CompilerException(string.Format(ErrorMessages.UnableToResolveData, identifier));
            }

            return data[identifier];
        }

        public static void Add(string identifier, ITypeDeclaration value)
        {
            types[identifier] = value;
        }

        public static void Add(string identifier, FunctorDeclaration value)
        {
            functors[identifier] = value;
        }

        public static void Add(string identifier, DataDeclaration value)
        {
            data[identifier] = value;
        }
    }
}
