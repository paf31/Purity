using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace Purity.Compiler.Data
{
    public class LFixTypeInfo : IFixPointInfo
    {
        public TypeBuilder Type
        {
            get;
            set;
        }

        public MethodBuilder Cata
        {
            get;
            set;
        }

        public MethodBuilder In
        {
            get;
            set;
        }

        public MethodBuilder Out
        {
            get;
            set;
        }

        public TypeBuilder CataFunction
        {
            get;
            set;
        }

        public ConstructorBuilder CataFunctionConstructor
        {
            get;
            set;
        }

        public TypeBuilder CataFunction1
        {
            get;
            set;
        }

        public ConstructorBuilder CataFunction1Constructor
        {
            get;
            set;
        }

        public TypeBuilder OutFunction
        {
            get;
            set;
        }

        public ConstructorBuilder OutFunctionConstructor
        {
            get;
            set;
        }

        public TypeBuilder OutClass
        {
            get;
            set;
        }

        public ConstructorBuilder OutClassConstructor
        {
            get;
            set;
        }

        public TypeBuilder InFunction 
        {
            get;
            set; 
        }

        public ConstructorBuilder InFunctionConstructor 
        {
            get; 
            set; 
        }
    }
}
