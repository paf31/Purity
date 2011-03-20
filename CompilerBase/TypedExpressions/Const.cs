using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Const : ITypedExpression
    {
        public ITypedExpression Value
        {
            get;
            set;
        }

        public IType InputType
        {
            get;
            set;
        }

        public IType OutputType
        {
            get;
            set;
        }

        public Const(ITypedExpression value, IType inputType, IType outputType)
        {
            Value = value;
            InputType = inputType;
            OutputType = outputType;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitConst(this);
        }

        public R AcceptVisitor<R>(ITypedExpressionVisitor<R> visitor)
        {
            return visitor.VisitConst(this);
        }
    }
}
