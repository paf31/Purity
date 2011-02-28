using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Ana : ITypedExpression
    {
        public ITypedExpression Coalgebra
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

        public Ana(ITypedExpression coalgebra, IType carrierType, IFunctor functor)
        {
            Coalgebra = coalgebra;
            CarrierType = carrierType;
            Functor = functor;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitAna(this);
        }
    }
}
