using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class DataSynonym : IConstrainedData
    {
        public string Identifier
        {
            get;
            set;
        }

        public IDictionary<string, IPartialType> TypeParameters
        {
            get;
            set;
        }

        public DataSynonym(string identifier, IDictionary<string, IPartialType> typeParameters)
        {
            Identifier = identifier;
            TypeParameters = typeParameters;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitSynonym(this);
        }

        public R AcceptVisitor<R>(IConstrainedDataVisitor<R> visitor)
        {
            return visitor.VisitSynonym(this);
        }
    }
}
