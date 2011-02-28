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

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitIdentity(this);
        }
    }
}
