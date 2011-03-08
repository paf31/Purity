using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using Purity.Core;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Functors
{
    public class ArrowFunctor : IFunctor
    {
        public IType Left { get; set; }

        public IFunctor Right { get; set; }

        public ArrowFunctor(IType left, IFunctor right)
        {
            this.Left = left;
            this.Right = right;
        }

        public void AcceptVisitor(Interfaces.IFunctorVisitor visitor)
        {
            visitor.VisitArrow(this);
        }
    }
}
