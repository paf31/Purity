using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Types
{
    public class ProductType : IType
    {
        public IType Left { get; set; }

        public IType Right { get; set; }

        public ProductType(IType left, IType right)
        {
            Left = left;
            Right = right;
        }

        public void AcceptVisitor(ITypeVisitor visitor)
        {
            visitor.VisitProduct(this);
        }
    }
}
