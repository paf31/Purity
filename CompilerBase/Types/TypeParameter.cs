using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Types
{
    public class TypeParameter : IType
    {
        public string Identifier { get; set; }

        public TypeParameter(string identifier)
        {
            Identifier = identifier;
        }

        public void AcceptVisitor(ITypeVisitor visitor)
        {
            visitor.VisitParameter(this);
        }

        public R AcceptVisitor<R>(ITypeVisitor<R> visitor)
        {
            return visitor.VisitParameter(this);
        }
    }
}
