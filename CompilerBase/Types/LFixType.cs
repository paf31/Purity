using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Functors;

namespace Purity.Compiler.Types
{
    public class LFixType : IType
    {
        public IFunctor Functor
        {
            get;
            set;
        }

        public LFixType(IFunctor functor)
        {
            Functor = functor;
        }

        public void AcceptVisitor(ITypeVisitor visitor)
        {
            visitor.VisitLFix(this);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                obj is LFixType &&
                (obj as LFixType).Functor.Equals(Functor);
        }

        public override string ToString()
        {
            return string.Format("l{0}", Functor);
        }
    }
}
