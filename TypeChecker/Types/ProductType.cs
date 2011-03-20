using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Types
{
    public class ProductType : IPartialType
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

        public ProductType(IPartialType left, IPartialType right)
        {
            Left = left;
            Right = right;
        }

        public void AcceptVisitor(IPartialTypeVisitor visitor)
        {
            visitor.VisitProduct(this);
        }

        public R AcceptVisitor<R>(IPartialTypeVisitor<R> visitor)
        {
            return visitor.VisitProduct(this);
        }
    }
}
