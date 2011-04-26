using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class Inl : IConstrainedData
    {
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

        public Inl(IPartialType leftType, IPartialType rightType)
        {
            this.LeftType = leftType;
            this.RightType = rightType;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitInl(this);
        }

        public R AcceptVisitor<R>(IConstrainedDataVisitor<R> visitor)
        {
            return visitor.VisitInl(this);
        }
    }
}
