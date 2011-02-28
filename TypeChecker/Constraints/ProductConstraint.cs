using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Constraints
{
    public class ProductConstraint : IConstraint
    {
        public int Left
        {
            get;
            set;
        }

        public int Right
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        public ProductConstraint(int left, int right, int index)
        {
            Left = left;
            Right = right;
            Index = index;
        }

        public void AcceptVisitor(IConstraintVisitor visitor)
        {
            visitor.VisitProduct(this);
        }
    }
}
