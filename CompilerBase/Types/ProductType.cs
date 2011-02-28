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

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                obj is ProductType &&
                (obj as ProductType).Left.Equals(Left) &&
                (obj as ProductType).Right.Equals(Right);
        }

        public override string ToString()
        {
            return string.Format("p{0}{1}", Left, Right);
        }
    }
}
