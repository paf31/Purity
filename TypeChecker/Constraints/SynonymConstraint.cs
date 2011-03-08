using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Constraints
{
    public class SynonymConstraint : IConstraint
    {
        public int Target
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        public SynonymConstraint(int target, int index)
        {
            Target = target;
            Index = index;
        }

        public void AcceptVisitor(IConstraintVisitor visitor)
        {
            visitor.VisitSynonym(this);
        }
    }
}
