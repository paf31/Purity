using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Functors
{
    public class IdentityFunctor : IPartialFunctor
    {
        public void AcceptVisitor(IPartialFunctorVisitor visitor)
        {
            visitor.VisitIdentity(this);
        }

        public R AcceptVisitor<R>(IPartialFunctorVisitor<R> visitor)
        {
            return visitor.VisitIdentity(this);
        }
    }
}
