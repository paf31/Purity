using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class PartialFunctorCreator : IFunctorVisitor<IPartialFunctor>
    {
        public static IPartialFunctor Convert(IFunctor functor)
        {
            return functor.AcceptVisitor(new PartialFunctorCreator());
        }

        public IPartialFunctor VisitArrow(Purity.Compiler.Functors.ArrowFunctor f)
        {
            return new Functors.ArrowFunctor(PartialTypeCreator.Convert(f.Left), Convert(f.Right));
        }

        public IPartialFunctor VisitConstant(Purity.Compiler.Functors.ConstantFunctor f)
        {
            return new Functors.ConstantFunctor(PartialTypeCreator.Convert(f.Value));
        }

        public IPartialFunctor VisitIdentity(Purity.Compiler.Functors.IdentityFunctor f)
        {
            return new Functors.IdentityFunctor();
        }

        public IPartialFunctor VisitProduct(Purity.Compiler.Functors.ProductFunctor f)
        {
            return new Functors.ProductFunctor(Convert(f.Left), Convert(f.Right));
        }

        public IPartialFunctor VisitSum(Purity.Compiler.Functors.SumFunctor f)
        {
            return new Functors.SumFunctor(Convert(f.Left), Convert(f.Right));
        }

        public IPartialFunctor VisitSynonym(Compiler.Functors.FunctorSynonym f)
        {
            return new Functors.FunctorSynonym(f.Identifier);
        }
    }
}
