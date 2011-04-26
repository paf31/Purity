using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class Identity : IConstrainedData
    {
        public IPartialType Type
        {
            get;
            set;
        }

        public Identity(IPartialType type)
        {
            Type = type;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitIdentity(this);
        }

        public R AcceptVisitor<R>(IConstrainedDataVisitor<R> visitor)
        {
            return visitor.VisitIdentity(this);
        }
    }
}
