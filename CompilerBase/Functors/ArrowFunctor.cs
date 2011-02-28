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

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                obj is ArrowFunctor &&
                (obj as ArrowFunctor).Left.Equals(Left) &&
                (obj as ArrowFunctor).Right.Equals(Right);
        }

        public override string ToString()
        {
            return string.Format("A{0}{1}", Left, Right);
        }
    }
}
