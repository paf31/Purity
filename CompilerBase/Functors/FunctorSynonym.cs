using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Functors;
using System.Reflection.Emit;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Functors
{
    public class FunctorSynonym : IFunctor
    {
        public string Identifier { get; set; }

        public FunctorSynonym(string identifier)
        {
            Identifier = identifier;
        }

        public void AcceptVisitor(Interfaces.IFunctorVisitor visitor)
        {
            visitor.VisitSynonym(this);
        }
    }
}
