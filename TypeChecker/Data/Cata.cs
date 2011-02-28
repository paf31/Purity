using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class Cata : IConstrainedData
    {
        public IConstrainedData Algebra
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

        public Cata(IConstrainedData algebra)
        {
            Algebra = algebra;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitCata(this);
        }
    }
}
