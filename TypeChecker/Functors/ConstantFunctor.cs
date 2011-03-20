using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using Purity.Core;
using Purity.Compiler.Types;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Functors
{
    public class ConstantFunctor : IPartialFunctor
    {
        public IPartialType Value
        {
            get;
            set;
        }

        public ConstantFunctor(IPartialType value)
        {
            Value = value;
        }

        public void AcceptVisitor(IPartialFunctorVisitor visitor)
        {
            visitor.VisitConstant(this);
        }

        public R AcceptVisitor<R>(IPartialFunctorVisitor<R> visitor)
        {
            return visitor.VisitConstant(this);
        }
    }
}
