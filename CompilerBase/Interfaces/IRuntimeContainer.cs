using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Purity.Compiler.Interfaces
{
    public interface IRuntimeContainer
    {
        void Clear();
        
        void Add(string name, MethodInfo value);

        void AddDestructor(string name, Type value);

        void Add(string name, Type type);
        
        MethodInfo Resolve(string name);
        
        Type ResolveDestructor(string name);

        Type ResolveType(string name);
    }
}
