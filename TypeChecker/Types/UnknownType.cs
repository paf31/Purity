using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Types
{
    public class UnknownType : IPartialType
    {
        public int Index
        {
            get;
            set;
        }

        public UnknownType(int index)
        {
            Index = index;
        }

        public void AcceptVisitor(IPartialTypeVisitor visitor)
        {
            visitor.VisitUnknown(this);
        }

        public R AcceptVisitor<R>(IPartialTypeVisitor<R> visitor)
        {
            return visitor.VisitUnknown(this);
        }
    }
}
