using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class Split : IConstrainedData
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

        public IPartialType RightType
        {
            get;
            set;
        }

        public IPartialType InputType
        {
            get;
            set;
        }

        public Split(IConstrainedData left, IConstrainedData right, IPartialType leftType, IPartialType rightType, IPartialType inputType)
        {
            Left = left;
            Right = right;
            LeftType = leftType;
            RightType = rightType;
            InputType = inputType;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitSplit(this);
        }

        public R AcceptVisitor<R>(IConstrainedDataVisitor<R> visitor)
        {
            return visitor.VisitSplit(this);
        }
    }
}
