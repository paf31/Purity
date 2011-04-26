using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class Application : IConstrainedData
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

        public Application(IConstrainedData left, IConstrainedData right, IPartialType leftType, IPartialType rightType)
        {
            Left = left;
            Right = right;
            LeftType = leftType;
            RightType = rightType;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitApplication(this);
        }

        public R AcceptVisitor<R>(IConstrainedDataVisitor<R> visitor)
        {
            return visitor.VisitApplication(this);
        }
    }
}
