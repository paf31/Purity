using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Core;
using System.Reflection.Emit;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Functors
{
    public class SumFunctor : IFunctor
    {
        public IFunctor Left { get; set; }

        public IFunctor Right { get; set; }

        public SumFunctor(IFunctor left, IFunctor right)
        {
            this.Left = left;
            this.Right = right;
        }

        public void AcceptVisitor(Interfaces.IFunctorVisitor visitor)
        {
            visitor.VisitSum(this);
        }
    }
}
