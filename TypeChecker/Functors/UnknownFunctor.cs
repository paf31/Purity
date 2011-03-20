using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Functors
{
    public class UnknownFunctor : IPartialFunctor
    {
        public UnknownFunctor(int index)
        {
            Index = index;
        }

        public int Index
        {
            get;
            set;
        }

        public void AcceptVisitor(IPartialFunctorVisitor visitor)
        {
            visitor.VisitUnknown(this);
        }

        public R AcceptVisitor<R>(IPartialFunctorVisitor<R> visitor)
        {
            return visitor.VisitUnknown(this);
        }
    }
}
