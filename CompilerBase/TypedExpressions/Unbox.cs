using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Unbox : ITypedExpression
    {
        public IType Target
        {
            get;
            set;
        }

        public IType Type
        {
            get;
            set;
        }

        public Unbox(IType target, IType type)
        {
            Target = target;
            Type = type;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitUnbox(this);
        }

        public R AcceptVisitor<R>(ITypedExpressionVisitor<R> visitor)
        {
            return visitor.VisitUnbox(this);
        }
    }
}
