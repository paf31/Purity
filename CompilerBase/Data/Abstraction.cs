using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Data
{
    public class Abstraction : IData
    {
        public string Variable
        {
            get;
            set;
        }

        public IData Body
        {
            get;
            set;
        }

        public Abstraction(string variable, IData body)
        {
            Variable = variable;
            Body = body;
        }

        public void AcceptVisitor(IDataVisitor visitor)
        {
            visitor.VisitAbstraction(this);
        }
    }
}
