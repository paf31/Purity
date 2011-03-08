using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class DataSynonym : ITypedExpression
    {
        public string Identifier { get; set; }

        public DataSynonym(string identifier)
        {
            Identifier = identifier;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitSynonym(this);
        }
    }
}
