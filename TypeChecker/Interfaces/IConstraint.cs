using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Typechecker.Interfaces
{
    public interface IConstraint
    {
        int Index
        {
            get;
            set;
        }

        void AcceptVisitor(IConstraintVisitor visitor);
    }
}
