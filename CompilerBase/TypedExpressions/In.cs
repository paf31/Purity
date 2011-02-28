using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class In : ITypedExpression
    {
        public IFunctor Functor
        {
            get;
            set;
        }
        public IType Source
        {
            get;
            set;
        }

        public In(IType source, IFunctor functor)
        {
            Source = source;
            Functor = functor;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitIn(this);
        }
    }
}
