using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Core;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Types
{
    public class TypeParameter : IPartialType
    {
        public string Identifier
        {
            get;
            set;
        }

        public TypeParameter(string identifier)
        {
            Identifier = identifier;
        }

        public void AcceptVisitor(IPartialTypeVisitor visitor)
        {
            visitor.VisitParameter(this);
        }

        public R AcceptVisitor<R>(IPartialTypeVisitor<R> visitor)
        {
            return visitor.VisitParameter(this);
        }
    }
}
