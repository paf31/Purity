using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace Purity.Compiler.Data
{
    public class BoxedTypeInfo : ITypeInfo
    {
        public Type Type
        {
            get;
            set;
        }

        public FieldBuilder Field
        {
            get;
            set;
        }

        public ConstructorBuilder Constructor
        {
            get;
            set;
        }

        public TypeBuilder BoxFunction
        {
            get;
            set;
        }

        public ConstructorBuilder BoxFunctionConstructor
        {
            get;
            set;
        }

        public TypeBuilder UnboxFunction
        {
            get;
            set;
        }

        public ConstructorBuilder UnboxFunctionConstructor
        {
            get;
            set;
        }
    }
}
