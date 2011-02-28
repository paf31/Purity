using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Core;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Types
{
    public class ArrowType : IType
    {
        public IType Left { get; set; }

        public IType Right { get; set; }

        public ArrowType(IType left, IType right)
        {
            Left = left;
            Right = right;
        }

        public void AcceptVisitor(ITypeVisitor visitor)
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
                obj is ArrowType &&
                (obj as ArrowType).Left.Equals(Left) &&
                (obj as ArrowType).Right.Equals(Right);
        }

        public override string ToString()
        {
            return string.Format("a{0}{1}", Left, Right);
        }
    }
}
