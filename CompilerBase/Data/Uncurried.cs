using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Data
{
    public class Uncurried : IData
    {
        public IData Function { get; set; }

        public Uncurried(IData function)
        {
            Function = function;
        }

        public void AcceptVisitor(IDataVisitor visitor)
        {
            visitor.VisitUncurry(this);
        }

        public IType Type
        {
            get;
            set;
        }
    }
}
