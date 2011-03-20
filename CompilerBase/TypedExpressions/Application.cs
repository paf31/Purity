using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Application : ITypedExpression
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

        public Application(ITypedExpression left, ITypedExpression right, IType leftType, IType rightType)
        {
            Left = left;
            Right = right;
            LeftType = leftType;
            RightType = rightType;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitApplication(this);
        }

        public R AcceptVisitor<R>(ITypedExpressionVisitor<R> visitor)
        {
            return visitor.VisitApplication(this);
        }
    }
}
