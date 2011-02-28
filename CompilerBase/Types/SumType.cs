using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Core;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Types
{
    public class SumType : IType
    {
        public IType Left { get; set; }

        public IType Right { get; set; }

        public SumType(IType left, IType right)
        {
            Left = left;
            Right = right;
        }

        public void AcceptVisitor(ITypeVisitor visitor)
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
                obj is SumType &&
                (obj as SumType).Left.Equals(Left) &&
                (obj as SumType).Right.Equals(Right);
        }

        public override string ToString()
        {
            return string.Format("s{0}{1}", Left, Right);
        }
    }
}
