using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Functors
{
    public class IdentityFunctor : IFunctor
    {
        public void AcceptVisitor(Interfaces.IFunctorVisitor visitor)
        {
            visitor.VisitIdentity(this);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                obj is IdentityFunctor;
        }
    }
}
