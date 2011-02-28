using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Functors;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Types
{
    public class LFixType : IPartialType
    {
        public IPartialFunctor Functor
        {
            get;
            set;
        }

        public LFixType(IPartialFunctor functor)
        {
            Functor = functor;
        }

        public void AcceptVisitor(IPartialTypeVisitor visitor)
        {
            visitor.VisitLFix(this);
        }
    }
}
