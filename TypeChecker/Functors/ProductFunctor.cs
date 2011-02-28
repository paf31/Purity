using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using Purity.Core;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Functors
{
    public class ProductFunctor : IPartialFunctor
    {
        public IPartialFunctor Left
        {
            get;
            set;
        }

        public IPartialFunctor Right
        {
            get;
            set;
        }

        public ProductFunctor(IPartialFunctor left, IPartialFunctor right)
        {
            this.Left = left;
            this.Right = right;
        }

        public void AcceptVisitor(IPartialFunctorVisitor visitor)
        {
            visitor.VisitProduct(this);
        }
    }
}
