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
        private static IDictionary<string, DataInfo> data =
            new Dictionary<string, DataInfo>();

        public static DataInfo Resolve(string name)
        {
            if (!data.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.UnableToResolveData, name.ToString())); 
            }

            return data[name];
        }

        public static void Add(string name, DataInfo value)
        {
            if (data.ContainsKey(name))
            {
                throw new CompilerException(string.Format(ErrorMessages.NameConflict, name));
            }

            data[name] = value;
        }

        public static void Clear()
        {
            data.Clear();
        }
    }
}
