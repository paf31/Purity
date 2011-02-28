using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Constraints
{
    public class FunctorAppConstraint : IConstraint
    {
        public int Functor
        {
            get;
            set;
        }

        public int Argument
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        public FunctorAppConstraint(int functor, int argument, int index)
        {
            Functor = functor;
            Argument = argument;
            Index = index;
        }

        public void AcceptVisitor(IConstraintVisitor visitor)
        {
            visitor.VisitFunctorApp(this);
        }
    }
}
