using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Modules;

namespace Purity.Compiler.Interfaces
{
    public interface IMetadataContainer
    {
        void Clear();

        void Add(string identifier, ITypeDeclaration value);

        void Add(string identifier, DataDeclaration value);

        ITypeDeclaration ResolveType(string identifier);

        DataDeclaration ResolveValue(string identifier);

        IEnumerable<KeyValuePair<string, DataDeclaration>> Values();
    }
}
