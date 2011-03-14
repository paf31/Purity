using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class Cata : IConstrainedData
    {
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

        public IPartialType LFixType
        {
            get;
            set;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitCata(this);
        }
    }
}
