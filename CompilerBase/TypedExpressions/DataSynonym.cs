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

        public IDictionary<string, IType> TypeParameters { get; set; }

        public DataSynonym(string identifier, IDictionary<string, IType> typeParameters)
        {
            Identifier = identifier;
            TypeParameters = typeParameters;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitSynonym(this);
        }

        public R AcceptVisitor<R>(ITypedExpressionVisitor<R> visitor)
        {
            return visitor.VisitSynonym(this);
        }
    }
}
