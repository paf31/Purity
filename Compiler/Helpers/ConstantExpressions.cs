using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Helpers
{
    public class ConstantExpressions : ITypedExpressionVisitor<bool>
    {
        private readonly string variableName;

        public ConstantExpressions(string variableName)
        {
            this.variableName = variableName;
        }

        public static bool IsConstantExpression(ITypedExpression expression, string variableName) 
        {
            return expression.AcceptVisitor(new ConstantExpressions(variableName));
        }

        public bool VisitConst(TypedExpressions.Const d)
        {
            return d.Value.AcceptVisitor(this);
        }

        public bool VisitIdentity(TypedExpressions.Identity d)
        {
            return true;
        }

        public bool VisitInl(TypedExpressions.Inl d)
        {
            return true;
        }

        public bool VisitInr(TypedExpressions.Inr d)
        {
            return true;
        }

        public bool VisitOutl(TypedExpressions.Outl d)
        {
            return true;
        }

        public bool VisitOutr(TypedExpressions.Outr d)
        {
            return true;
        }

        public bool VisitUncurry(TypedExpressions.Uncurried d)
        {
            return d.Function.AcceptVisitor(this);
        }

        public bool VisitCurry(TypedExpressions.Curried d)
        {
            return d.Function.AcceptVisitor(this);
        }

        public bool VisitSynonym(TypedExpressions.DataSynonym d)
        {
            return true;
        }

        public bool VisitApplication(TypedExpressions.Application d)
        {
            return d.Left.AcceptVisitor(this) && d.Right.AcceptVisitor(this);
        }

        public bool VisitCase(TypedExpressions.Case d)
        {
            return d.Left.AcceptVisitor(this) && d.Right.AcceptVisitor(this);
        }

        public bool VisitComposition(TypedExpressions.Composition d)
        {
            return d.Left.AcceptVisitor(this) && d.Right.AcceptVisitor(this);
        }

        public bool VisitSplit(TypedExpressions.Split d)
        {
            return d.Left.AcceptVisitor(this) && d.Right.AcceptVisitor(this);
        }

        public bool VisitAbstraction(TypedExpressions.Abstraction d)
        {
            return d.Body.AcceptVisitor(this);
        }

        public bool VisitVariable(TypedExpressions.Variable d)
        {
            return !d.Name.Equals(variableName);
        }
    }
}
