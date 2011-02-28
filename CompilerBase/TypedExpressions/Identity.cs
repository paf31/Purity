using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Identity : ITypedExpression
    {
        public IType Type
        {
            get;
            set;
        }

        public Identity(IType type)
        {
            Type = type;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitIdentity(this);
        }
    }
}
