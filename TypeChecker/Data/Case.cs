using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class Case : IConstrainedData
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

        public IPartialType ResultType
        {
            get;
            set;
        }

        public Case(IConstrainedData left, IConstrainedData right, IPartialType leftType, IPartialType rightType, IPartialType resultType)
        {
            Left = left;
            Right = right;
            LeftType = leftType;
            RightType = rightType;
            ResultType = resultType;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitCase(this);
        }

        public R AcceptVisitor<R>(IConstrainedDataVisitor<R> visitor)
        {
            return visitor.VisitCase(this);
        }
    }
}
