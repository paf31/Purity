using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Outl : ITypedExpression
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

        public Outl(IType leftType, IType rightType)
        {
            LeftType = leftType;
            RightType = rightType;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitOutl(this);
        }
    }
}
