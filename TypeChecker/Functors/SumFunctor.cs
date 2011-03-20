using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Core;
using System.Reflection.Emit;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Functors
{
    public class SumFunctor : IPartialFunctor
    {
        public IPartialFunctor Left
        {
            get;
            set;
        }

        public IPartialFunctor Right
        {
            get;
            set;
        }

        public SumFunctor(IPartialFunctor left, IPartialFunctor right)
        {
            this.Left = left;
            this.Right = right;
        }

        public void AcceptVisitor(IPartialFunctorVisitor visitor)
        {
            visitor.VisitSum(this);
        }

        public R AcceptVisitor<R>(IPartialFunctorVisitor<R> visitor)
        {
            return visitor.VisitSum(this);
        }
    }
}
