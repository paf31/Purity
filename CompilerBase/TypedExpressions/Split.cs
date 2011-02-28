using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Split : ITypedExpression
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

        public IType InputType
        {
            get;
            set;
        }

        public Split(ITypedExpression left, ITypedExpression right, IType leftType, IType rightType, IType inputType)
        {
            Left = left;
            Right = right;
            LeftType = leftType;
            RightType = rightType;
            InputType = inputType;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitSplit(this);
        }
    }
}
