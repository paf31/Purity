using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Box : ITypedExpression
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
        
        public Box(IType target, IType type)
        {
            Target = target;
            Type = type;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitBox(this);
        }

        public R AcceptVisitor<R>(ITypedExpressionVisitor<R> visitor)
        {
            return visitor.VisitBox(this);
        }
    }
}
