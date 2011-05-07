using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Data;
using Purity.Compiler.TypedExpressions;
using Purity.Compiler.Modules;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Implementation
{
    public class MetadataContainer : IMetadataContainer
    {
        private readonly IDictionary<string, ITypeDeclaration> types =
            new Dictionary<string, ITypeDeclaration>();

        private readonly IDictionary<string, DataDeclaration> data =
            new Dictionary<string, DataDeclaration>();

        public ITypeDeclaration ResolveType(string identifier)
        {
            if (!types.ContainsKey(identifier))
            {
                throw new CompilerException(string.Format(ErrorMessages.UnableToResolveType, identifier));
            }

            return types[identifier];
        }

        public DataDeclaration ResolveValue(string identifier)
        {
            if (!data.ContainsKey(identifier))
            {
                throw new CompilerException(string.Format(ErrorMessages.UnableToResolveData, identifier));
            }

            return data[identifier];
        }

        public IEnumerable<KeyValuePair<string, DataDeclaration>> Values()
        {
            return data;
        }

        public void Add(string identifier, ITypeDeclaration value)
        {
            types[identifier] = value;
        }

        public void Add(string identifier, DataDeclaration value)
        {
            data[identifier] = value;
        }

        public void Clear()
        {
            data.Clear();
            types.Clear();
        }
    }
}
