using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Data
{
    public class Variable : IData
    {
        public string Name
        {
            get;
            set;
        }

        public Variable(string name)
        {
            Name = name;
        }

        public void AcceptVisitor(IDataVisitor visitor)
        {
            visitor.VisitVariable(this);
        }

        public R AcceptVisitor<R>(IDataVisitor<R> visitor)
        {
            return visitor.VisitVariable(this);
        }
    }
}
