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
    public static class DataContainer
    {
        private static IDictionary<string, MethodInfo> data =
            new Dictionary<string, MethodInfo>();
        private static IDictionary<string, Type> destructors =
            new Dictionary<string, Type>();

        public static MethodInfo Resolve(string name)
        {
            if (!data.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.UnableToResolveData, name.ToString())); 
            }

            return data[name];
        }

        public static Type ResolveDestructor(string name)
        {
            if (!destructors.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.UnableToResolveData, name.ToString()));
            }

            return destructors[name];
        }

        public static void Add(string name, MethodInfo value)
        {
            if (data.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.NameConflict, name));
            }

            data[name] = value;
        }

        public static void AddDestructor(string name, Type value)
        {
            if (destructors.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.NameConflict, name));
            }

            destructors[name] = value;
        }

        public static void Clear()
        {
            data.Clear();
            destructors.Clear();
        }
    }
}
