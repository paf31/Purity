using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Data
{
    public class DataSynonym : IData
    {
        public string Identifier { get; set; }

        public DataSynonym(string identifier)
        {
            Identifier = identifier;
        }

        public void AcceptVisitor(IDataVisitor visitor)
        {
            visitor.VisitSynonym(this);
        }

        public IType Type
        {
            get;
            set;
        }
    }
}
