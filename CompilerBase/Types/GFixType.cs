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

        public string Identifier
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
    }
}
