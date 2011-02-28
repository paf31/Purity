using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Out : ITypedExpression
    {
        public IFunctor Functor
        {
            get;
            set;
        }

        public IType Target
        {
            get;
            set;
        }

        public Out(IType target, IFunctor functor)
        {
            Target = target;
            Functor = functor;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitOut(this);
        }
    }
}
