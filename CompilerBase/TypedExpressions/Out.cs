using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Out : ITypedExpression
    {
        public IType Target
        {
            get;
            set;
        }

        public Out(IType target)
        {
            Target = target;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitOut(this);
        }
    }
}
