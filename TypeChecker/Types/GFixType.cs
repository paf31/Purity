using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Functors;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Types
{
    public class GFixType : IPartialType
    {
        public IPartialFunctor Functor
        {
            get;
            set;
        }

        public string Identifier
        {
            get;
            set;
        }

        public GFixType(IPartialFunctor functor)
        {
            Functor = functor;
        }

        public void AcceptVisitor(IPartialTypeVisitor visitor)
        {
            visitor.VisitGFix(this);
        }

        public R AcceptVisitor<R>(IPartialTypeVisitor<R> visitor)
        {
            return visitor.VisitGFix(this);
        }
    }
}
