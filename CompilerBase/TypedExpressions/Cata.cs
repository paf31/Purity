using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Cata : ITypedExpression
    {
        public ITypedExpression Algebra
        {
            get;
            set;
        }

        public IType CarrierType
        {
            get;
            set;
        }

        public IFunctor Functor
        {
            get;
            set;
        }

        public Cata(ITypedExpression algebra, IType carrierType, IFunctor functor)
        {
            Algebra = algebra;
            CarrierType = carrierType;
            Functor = functor;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitCata(this);
        }
    }
}
