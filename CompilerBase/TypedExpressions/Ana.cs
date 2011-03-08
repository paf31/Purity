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


        public IType GFixType
        {
            get;
            set;
        }

        public Ana(ITypedExpression coalgebra, IType carrierType, IType gfixType)
        {
            Coalgebra = coalgebra;
            CarrierType = carrierType;
            GFixType = gfixType;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitAna(this);
        }
    }
}
