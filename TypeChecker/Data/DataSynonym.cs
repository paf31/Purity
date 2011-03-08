using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class DataSynonym : IConstrainedData
    {
        public DataSynonym(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier
        {
            get;
            set;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitSynonym(this);
        }
    }
}
