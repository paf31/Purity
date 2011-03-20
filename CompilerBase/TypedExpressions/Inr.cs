using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Inr : ITypedExpression
    {
        public IType LeftType
        {
            get;
            set;
        }

        public IType RightType
        {
            get;
            set;
        }

        public Inr(IType leftType, IType rightType)
        {
            LeftType = leftType;
            RightType = rightType;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitInr(this);
        }

        public R AcceptVisitor<R>(ITypedExpressionVisitor<R> visitor)
        {
            return visitor.VisitInr(this);
        }
    }
}
