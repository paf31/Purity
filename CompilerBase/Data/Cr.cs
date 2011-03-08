using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Data
{
    public class Cr : IData
    {
        public void AcceptVisitor(IDataVisitor visitor)
        {
            visitor.VisitCr(this);
        }

        public Cr(IData function)
        {
            Function = function;
        }

        public IData Function
        {
            get;
            set;
        }
    }
}
