using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using Purity.Core;
using Purity.Compiler.Types;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Functors
{
    public class ConstantFunctor : IFunctor
    {
        public IType Value
        {
            get;
            set;
        }

        public ConstantFunctor(IType value)
        {
            Value = value;
        }

        public void AcceptVisitor(Interfaces.IFunctorVisitor visitor)
        {
            visitor.VisitConstant(this);
        }
    }
}
