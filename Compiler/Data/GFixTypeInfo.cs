using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace Purity.Compiler.Data
{
    public class GFixTypeInfo : IFixPointInfo
    {
        public TypeBuilder Type
        {
            get;
            set;
        }

        public TypeBuilder GreatestFixedPointFunction
        {
            get;
            set;
        }

        public MethodBuilder GreatestFixedPointFunctionApplyMethod
        {
            get;
            set;
        }

        public MethodBuilder Ana
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

        public TypeBuilder AnaClass
        {
            get;
            set;
        }

        public ConstructorBuilder AnaClassConstructor
        {
            get;
            set;
        }

        public TypeBuilder AnaFunction
        {
            get;
            set;
        }

        public ConstructorBuilder AnaFunctionConstructor
        {
            get;
            set;
        }

        public ConstructorBuilder InGeneratingFunctionConstructor
        {
            get;
            set;
        }

        public ConstructorBuilder InFunctionConstructor
        {
            get;
            set;
        }

        public MethodBuilder GreatestFixedPointApplyMethod
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

        public TypeBuilder InFunction 
        {
            get; 
            set; 
        }
    }
}
