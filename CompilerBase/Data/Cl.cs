using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Data
{
    public class Cl : IData
    {
        public void AcceptVisitor(IDataVisitor visitor)
        {
            visitor.VisitCl(this);
        }

        public Cl(IData function)
        {
            Function = function;
        }

        public IData Function
        {
            get;
            set;
        }

        public IType Type
        {
            get;
            set;
        }
    }
}
