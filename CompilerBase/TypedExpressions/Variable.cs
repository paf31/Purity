using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Variable : ITypedExpression
    {
        public string Name
        {
            get;
            set;
        }

        public IType Type
        {
            get;
            set;
        }

        public Variable(string name, IType type)
        {
            Name = name;
            Type = type;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitVariable(this);
        }
    }
}
