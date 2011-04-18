using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace Purity.Compiler.Data
{
    public interface IFixPointInfo : ITypeInfo
    {
        MethodBuilder In
        {
            get;
            set;
        }

        MethodBuilder Out
        {
            get;
            set;
        }

        TypeBuilder InFunction
        {
            get;
            set;
        }

        TypeBuilder OutFunction
        {
            get;
            set;
        }

        ConstructorBuilder OutFunctionConstructor
        {
            get;
            set;
        }

        ConstructorBuilder InFunctionConstructor
        {
            get;
            set;
        }
    }
}
