using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Data
{
    public class Const : IData
    {
        public IData Value
        {
            get;
            set;
        }

        public Const(IData value)
        {
            Value = value;
        }

        public void AcceptVisitor(IDataVisitor visitor)
        {
            visitor.VisitConst(this);
        }

        public IType Type
        {
            get;
            set;
        }
    }
}
