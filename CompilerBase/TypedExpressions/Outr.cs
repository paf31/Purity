using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Outr : ITypedExpression
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

        public Outr(IType leftType, IType rightType)
        {
            LeftType = leftType;
            RightType = rightType;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitOutr(this);
        }

        public R AcceptVisitor<R>(ITypedExpressionVisitor<R> visitor)
        {
            return visitor.VisitOutr(this);
        }
    }
}
