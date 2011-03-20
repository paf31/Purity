using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using Purity.Core;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Functors
{
    public class ProductFunctor : IFunctor
    {
        public IFunctor Left { get; set; }

        public IFunctor Right { get; set; }

        public ProductFunctor(IFunctor left, IFunctor right)
        {
            this.Left = left;
            this.Right = right;
        }

        public void AcceptVisitor(Interfaces.IFunctorVisitor visitor)
        {
            visitor.VisitProduct(this);
        }

        public R AcceptVisitor<R>(Interfaces.IFunctorVisitor<R> visitor)
        {
            return visitor.VisitProduct(this);
        }
    }
}
