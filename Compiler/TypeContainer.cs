using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Data;
using System.Reflection;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler
{
    public static class TypeContainer
    {
        private static IDictionary<string, ITypeInfo> types =
            new Dictionary<string, ITypeInfo>();

        public static LFixTypeInfo ResolveLFixType(string name)
        {
            if (!types.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.UnableToResolveLFix, name.ToString()));
            }

            return (LFixTypeInfo) types[name];
        }

        public static GFixTypeInfo ResolveGFixType(string name)
        {
            if (!types.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.UnableToResolveGFix, name.ToString()));
            }

            return (GFixTypeInfo) types[name];
        }

        public static IFixPointInfo ResolveFixPointType(string name)
        {
            if (!types.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.UnableToResolveFix, name.ToString()));
            }

            return (IFixPointInfo) types[name];
        }

        public static ITypeInfo ResolveType(string name)
        {
            if (!types.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.UnableToResolveType, name.ToString()));
            }

            return (ITypeInfo) types[name];
        }

        public static void Add(string name, ITypeInfo value)
        {
            if (types.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.NameConflict, name));
            }

            types[name] = value;
        }
    }
}
