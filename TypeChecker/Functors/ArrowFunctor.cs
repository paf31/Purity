using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using Purity.Core;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Functors
{
    public class ArrowFunctor : IPartialFunctor
    {
        public IPartialType Left
        {
            get;
            set;
        }

        public IPartialFunctor Right
        {
            get;
            set;
        }

        public ArrowFunctor(IPartialType left, IPartialFunctor right)
        {
            this.Left = left;
            this.Right = right;
        }

        public void AcceptVisitor(IPartialFunctorVisitor visitor)
        {
            visitor.VisitArrow(this);
        }
    }
}
