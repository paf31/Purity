using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Composition : ITypedExpression
    {
        public ITypedExpression Left
        {
            get;
            set;
        }

        public ITypedExpression Right
        {
            get;
            set;
        }

        public IType LeftType
        {
            get;
            set;
        }

        public IType MiddleType
        {
            get;
            set;
        }

        public IType RightType
        {
            get;
            set;
        }

        public Composition(ITypedExpression left, ITypedExpression right, IType leftType, IType middleType, IType rightType)
        {
            Left = left;
            Right = right;
            LeftType = leftType;
            MiddleType = middleType;
            RightType = rightType;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitComposition(this);
        }

        public R AcceptVisitor<R>(ITypedExpressionVisitor<R> visitor)
        {
            return visitor.VisitComposition(this);
        }
    }
}
