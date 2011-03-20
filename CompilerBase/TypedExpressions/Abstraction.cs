using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Abstraction : ITypedExpression
    {
        public string Variable
        {
            get;
            set;
        }

        public ITypedExpression Body
        {
            get;
            set;
        }

        public IType BodyType
        {
            get;
            set;
        }

        public IType VariableType
        {
            get;
            set;
        }

        public ITypedExpression PointFreeExpression
        {
            get;
            set;
        }

        public Abstraction(string variable, ITypedExpression body, IType bodyType, IType variableType)
        {
            Variable = variable;
            Body = body;
            BodyType = bodyType;
            VariableType = variableType;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitAbstraction(this);
        }

        public R AcceptVisitor<R>(ITypedExpressionVisitor<R> visitor)
        {
            return visitor.VisitAbstraction(this);
        }
    }
}
