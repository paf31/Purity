using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class In : IConstrainedData
    {
        public IPartialFunctor Functor
        {
            get;
            set;
        }

        public IPartialType Source
        {
            get;
            set;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitIn(this);
        }
    }
}
