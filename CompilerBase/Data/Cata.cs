using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Data
{
    public class Cata : IData
    {
        public IData Algebra
        {
            get;
            set;
        }

        public Cata(IData algebra)
        {
            Algebra = algebra;
        }

        public void AcceptVisitor(IDataVisitor visitor)
        {
            visitor.VisitCata(this);
        }

        public IType Type
        {
            get;
            set;
        }
    }
}
