using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Functors;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Types
{
    public class TypeSynonym : IType
    {
        public string Identifier { get; set; }

        public TypeSynonym(string identifier)
        {
            Identifier = identifier;
        }

        public void AcceptVisitor(ITypeVisitor visitor)
        {
            visitor.VisitSynonym(this);
        }
    }
}
