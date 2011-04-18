using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Types;

namespace Purity.Compiler.Modules
{
    public class FunctorDeclaration
    {
        public IFunctor Functor
        {
            get;
            set;
        }

        public string[] TypeParameters
        {
            get;
            set;
        }

        public FunctorDeclaration(IFunctor functor, string[] typeParameters)
        {
            Functor = functor;
            TypeParameters = typeParameters;
        }
    }
}
