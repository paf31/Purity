using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Helpers
{
    public class AbstractionElimination : ITypedExpressionVisitor
    {
        private IMetadataContainer container;

        public AbstractionElimination(IMetadataContainer container) 
        {
            this.container = container;
        }

        public void VisitApplication(TypedExpressions.Application d)
        {
            d.Left.AcceptVisitor(this);
            d.Right.AcceptVisitor(this);
        }

        public void VisitCase(TypedExpressions.Case d)
        {
            d.Left.AcceptVisitor(this);
            d.Right.AcceptVisitor(this);
        }

        public void VisitComposition(TypedExpressions.Composition d)
        {
            d.Left.AcceptVisitor(this);
            d.Right.AcceptVisitor(this);
        }

        public void VisitConst(TypedExpressions.Const d)
        {
            d.Value.AcceptVisitor(this);
        }

        public void VisitIdentity(TypedExpressions.Identity d)
        {
        }

        public void VisitInl(TypedExpressions.Inl d)
        {
        }

        public void VisitInr(TypedExpressions.Inr d)
        {
        }

        public void VisitOutl(TypedExpressions.Outl d)
        {
        }

        public void VisitOutr(TypedExpressions.Outr d)
        {
        }

        public void VisitSplit(TypedExpressions.Split d)
        {
            d.Left.AcceptVisitor(this);
            d.Right.AcceptVisitor(this);
        }

        public void VisitUncurry(TypedExpressions.Uncurried d)
        {
            d.Function.AcceptVisitor(this);
        }

        public void VisitCurry(TypedExpressions.Curried d)
        {
            d.Function.AcceptVisitor(this);
        }

        public void VisitSynonym(TypedExpressions.DataSynonym d)
        {
        }

        public void VisitAbstraction(TypedExpressions.Abstraction d)
        {
            d.Body.AcceptVisitor(this);
            d.PointFreeExpression = VariableElimination.Visit(d.Body, d.Variable, d.VariableType, container);
        }

        public void VisitVariable(TypedExpressions.Variable d)
        {
        }
    }
}
