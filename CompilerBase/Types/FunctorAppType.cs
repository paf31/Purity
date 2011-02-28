using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Core;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Types
{
    public class FunctorAppType : IType
    {
        public IFunctor Functor { get; set; }

        public IType Argument { get; set; }

        public FunctorAppType(IFunctor functor, IType argument)
        {
            Functor = functor;
            Argument = argument;
        }

        public void AcceptVisitor(ITypeVisitor visitor)
        {
            visitor.VisitFunctorApp(this);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                obj is FunctorAppType &&
                (obj as FunctorAppType).Functor.Equals(Functor) &&
                (obj as FunctorAppType).Argument.Equals(Argument);
        }

        public override string ToString()
        {
            return string.Format("f{0}{1}", Functor, Argument);
        }
    }
}
