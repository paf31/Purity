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
        private static IDictionary<string, Type> types =
            new Dictionary<string, Type>();

        public static Type ResolveType(string name)
        {
            if (!types.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.UnableToResolveType, name.ToString()));
            }

            return types[name];
        }

        public static void Add(string name, Type type)
        {
            if (types.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.NameConflict, name));
            }

            types[name] = type;
        }

        public static void Clear()
        {
            types.Clear();
        }
    }
}
