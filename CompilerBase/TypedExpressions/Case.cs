using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Case : ITypedExpression
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

        public IType RightType
        {
            get;
            set;
        }

        public IType ResultType
        {
            get;
            set;
        }

        public Case(ITypedExpression left, ITypedExpression right, IType leftType, IType rightType, IType resultType)
        {
            Left = left;
            Right = right;
            LeftType = leftType;
            RightType = rightType;
            ResultType = resultType;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitCase(this);
        }

        public R AcceptVisitor<R>(ITypedExpressionVisitor<R> visitor)
        {
            return visitor.VisitCase(this);
        }
    }
}
