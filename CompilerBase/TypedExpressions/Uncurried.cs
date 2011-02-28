using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Uncurried : ITypedExpression
    {
        public ITypedExpression Function { get; set; }

        public Uncurried(ITypedExpression function, IType first, IType second, IType output)
        {
            Function = function;
            First = first;
            Second = second;
            Output = output;
        }

        public IType First { get; set; }

        public IType Second { get; set; }

        public IType Output { get; set; }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitUncurry(this);
        }
    }
}
