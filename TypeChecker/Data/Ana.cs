using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class Ana : IConstrainedData
    {
        public IConstrainedData Coalgebra
        {
            get;
            set;
        }

        public IPartialType CarrierType
        {
            get;
            set;
        }

        public IPartialFunctor Functor
        {
            get;
            set;
        }

        public Ana(IConstrainedData coalgebra)
        {
            Coalgebra = coalgebra;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitAna(this);
        }
    }
}
