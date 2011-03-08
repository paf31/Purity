using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class Box : IConstrainedData
    {
        public IPartialType Type
        {
            get;
            set;
        }

        public IPartialType Target
        {
            get;
            set;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitBox(this);
        }
    }
}
