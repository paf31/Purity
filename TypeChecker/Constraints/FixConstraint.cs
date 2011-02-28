using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Constraints
{
    public class FixConstraint : IConstraint
    {
        public int Functor
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        public FixConstraint(int functor, int index)
        {
            Functor = functor;
            Index = index;
        }

        public void AcceptVisitor(IConstraintVisitor visitor)
        {
            visitor.VisitFix(this);
        }
    }
}
