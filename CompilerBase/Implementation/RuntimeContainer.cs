using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Data;
using System.Reflection;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Implementation
{
    public class RuntimeContainer : IRuntimeContainer
    {
        private readonly IDictionary<string, MethodInfo> data =
            new Dictionary<string, MethodInfo>();
        private readonly IDictionary<string, Type> destructors =
            new Dictionary<string, Type>();
        private readonly IDictionary<string, Type> types =
            new Dictionary<string, Type>();

        public Type ResolveType(string name)
        {
            if (!types.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.UnableToResolveType, name.ToString()));
            }

            return types[name];
        }

        public MethodInfo Resolve(string name)
        {
            if (!data.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.UnableToResolveData, name.ToString())); 
            }

            return data[name];
        }

        public Type ResolveDestructor(string name)
        {
            if (!destructors.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.UnableToResolveData, name.ToString()));
            }

            return destructors[name];
        }

        public void Add(string name, MethodInfo value)
        {
            if (data.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.NameConflict, name));
            }

            data[name] = value;
        }

        public void AddDestructor(string name, Type value)
        {
            if (destructors.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.NameConflict, name));
            }

            destructors[name] = value;
        }

        public void Add(string name, Type type)
        {
            if (types.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.NameConflict, name));
            }

            types[name] = type;
        }

        public void Clear()
        {
            data.Clear();
            types.Clear();
            destructors.Clear();
        }
    }
}
