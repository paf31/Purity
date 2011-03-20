using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Curried : ITypedExpression
    {
        public ITypedExpression Function { get; set; }

        public IType First { get; set; }

        public IType Second { get; set; }

        public IType Output { get; set; }

        public Curried(ITypedExpression function, IType first, IType second, IType output)
        {
            Function = function;
            First = first;
            Second = second;
            Output = output;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitCurry(this);
        }

        public R AcceptVisitor<R>(ITypedExpressionVisitor<R> visitor)
        {
            return visitor.VisitCurry(this);
        }
    }
}
