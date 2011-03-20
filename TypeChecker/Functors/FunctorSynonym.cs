using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Functors
{
    public class FunctorSynonym : IPartialFunctor
    {
        public string Identifier { get; set; }

        public FunctorSynonym(string identifier)
        {
            Identifier = identifier;
        }
    
        public void  AcceptVisitor(IPartialFunctorVisitor visitor)
        {
            visitor.VisitSynonym(this);
        }

        public R AcceptVisitor<R>(IPartialFunctorVisitor<R> visitor)
        {
            return visitor.VisitSynonym(this);
        }
    }
}
