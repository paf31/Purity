using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Functors;

namespace Purity.Compiler.Types
{
    public class GFixType : IType
    {
        public IFunctor Functor
        {
            get;
            set;
        }

        public GFixType(IFunctor functor)
        {
            Functor = functor;
        }

        public void AcceptVisitor(ITypeVisitor visitor)
        {
            visitor.VisitGFix(this);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                obj is GFixType &&
                (obj as GFixType).Functor.Equals(Functor);
        }

        public override string ToString()
        {
            return string.Format("g{0}", Functor);
        }
    }
}
