using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Inl : ITypedExpression
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

        public Inl(IType leftType, IType rightType)
        {
            LeftType = leftType;
            RightType = rightType;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitInl(this);
        }

        public R AcceptVisitor<R>(ITypedExpressionVisitor<R> visitor)
        {
            return visitor.VisitInl(this);
        }
    }
}
