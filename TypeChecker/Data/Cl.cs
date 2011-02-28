using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class Cl : IConstrainedData
    {
        public IConstrainedData Function { get; set; }

        public Cl(IConstrainedData function)
        {
            Function = function;
        }

        public IPartialType A { get; set; }

        public IPartialType B { get; set; }

        public IPartialType C { get; set; }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitCl(this);
        }
    }
}
