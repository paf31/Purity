using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Core;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Types
{
    public class ArrowType : IPartialType
    {
        public IPartialType Left
        {
            get;
            set;
        }

        public IPartialType Right
        {
            get;
            set;
        }

        public ArrowType(IPartialType left, IPartialType right)
        {
            Left = left;
            Right = right;
        }

        public void AcceptVisitor(IPartialTypeVisitor visitor)
        {
            visitor.VisitArrow(this);
        }

        public R AcceptVisitor<R>(IPartialTypeVisitor<R> visitor)
        {
            return visitor.VisitArrow(this);
        }
    }
}
