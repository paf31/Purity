using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Ana : ITypedExpression
    {
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

        public Ana(IType carrierType, IType gfixType)
        {
            CarrierType = carrierType;
            GFixType = gfixType;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitAna(this);
        }

        public R AcceptVisitor<R>(ITypedExpressionVisitor<R> visitor)
        {
            return visitor.VisitAna(this);
        }
    }
}
