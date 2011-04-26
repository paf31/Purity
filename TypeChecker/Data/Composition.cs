using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class Composition : IConstrainedData
    {
        public IConstrainedData Left
        {
            get;
            set;
        }

        public IConstrainedData Right
        {
            get;
            set;
        }

        public IPartialType LeftType
        {
            get;
            set;
        }

        public IPartialType MiddleType
        {
            get;
            set;
        }

        public IPartialType RightType
        {
            get;
            set;
        }

        public Composition(IConstrainedData left, IConstrainedData right, IPartialType leftType, IPartialType middleType, IPartialType rightType)
        {
            Left = left;
            Right = right;
            LeftType = leftType;
            MiddleType = middleType;
            RightType = rightType;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitComposition(this);
        }

        public R AcceptVisitor<R>(IConstrainedDataVisitor<R> visitor)
        {
            return visitor.VisitComposition(this);
        }
    }
}
