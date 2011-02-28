using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Data
{
    public class Curried : IData
    {
        public IData Function { get; set; }

        public Curried(IData function)
        {
            Function = function;
        }

        public void AcceptVisitor(IDataVisitor visitor)
        {
            visitor.VisitCurry(this);
        }

        public IType Type
        {
            get;
            set;
        }
    }
}
