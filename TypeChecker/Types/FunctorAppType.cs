using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Core;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Types
{
    public class FunctorAppType : IPartialType
    {
        public IPartialFunctor Functor { get; set; }

        public IPartialType Argument
        {
            get;
            set;
        }

        public FunctorAppType(IPartialFunctor functor, IPartialType argument)
        {
            Functor = functor;
            Argument = argument;
        }

        public void AcceptVisitor(IPartialTypeVisitor visitor)
        {
            visitor.VisitFunctorApp(this);
        }
    }
}
