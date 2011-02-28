using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Cr : ITypedExpression
    {
        public ITypedExpression Function { get; set; }

        public Cr(ITypedExpression function, IType a, IType b, IType c)
        {
            Function = function;
            A = a;
            B = b;
            C = c;
        }

        public IType A { get; set; }

        public IType B { get; set; }

        public IType C { get; set; }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitCr(this);
        }
    }
}
