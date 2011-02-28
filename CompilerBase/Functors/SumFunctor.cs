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

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                obj is SumFunctor &&
                (obj as SumFunctor).Left.Equals(Left) &&
                (obj as SumFunctor).Right.Equals(Right);
        }

        public override string ToString()
        {
            return string.Format("S{0}{1}", Left, Right);
        }
    }
}
