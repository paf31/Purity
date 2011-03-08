using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Data
{
    public class Ana : IData
    {
        public IData Coalgebra
        {
            get;
            set;
        }

        public Ana(IData coalgebra)
        {
            Coalgebra = coalgebra;
        }

        public void AcceptVisitor(IDataVisitor visitor)
        {
            visitor.VisitAna(this);
        }
    }
}
