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
        public string Identifier
        {
            get;
            set;
        }

        public IType[] TypeParameters
        {
            get;
            set;
        }

        public TypeSynonym(string identifier, IType[] typeParameters)
        {
            Identifier = identifier;
            TypeParameters = typeParameters;
        }

        public void AcceptVisitor(ITypeVisitor visitor)
        {
            visitor.VisitSynonym(this);
        }

        public R AcceptVisitor<R>(ITypeVisitor<R> visitor)
        {
            return visitor.VisitSynonym(this);
        }
    }
}
