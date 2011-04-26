using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class Variable : IConstrainedData
    {
        public string Name
        {
            get;
            set;
        }

        public IPartialType Type
        {
            get;
            set;
        }

        public Variable(string name, IPartialType type)
        {
            Name = name;
            Type = type;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitVariable(this);
        }

        public R AcceptVisitor<R>(IConstrainedDataVisitor<R> visitor)
        {
            return visitor.VisitVariable(this);
        }
    }
}
