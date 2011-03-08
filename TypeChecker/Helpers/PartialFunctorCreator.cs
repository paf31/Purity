using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class PartialFunctorCreator : IFunctorVisitor
    {
        public IPartialFunctor Result
        {
            get;
            set;
        }

        public IPartialFunctor Convert(IFunctor functor)
        {
            functor.AcceptVisitor(this);
            return Result;
        }

        public void VisitArrow(Purity.Compiler.Functors.ArrowFunctor f)
        {
            Result = new Functors.ArrowFunctor(new PartialTypeCreator().Convert(f.Left), Convert(f.Right));
        }

        public void VisitConstant(Purity.Compiler.Functors.ConstantFunctor f)
        {
            Result = new Functors.ConstantFunctor(new PartialTypeCreator().Convert(f.Value));
        }

        public void VisitIdentity(Purity.Compiler.Functors.IdentityFunctor f)
        {
            Result = new Functors.IdentityFunctor();
        }

        public void VisitProduct(Purity.Compiler.Functors.ProductFunctor f)
        {
            Result = new Functors.ProductFunctor(Convert(f.Left), Convert(f.Right));
        }

        public void VisitSum(Purity.Compiler.Functors.SumFunctor f)
        {
            Result = new Functors.SumFunctor(Convert(f.Left), Convert(f.Right));
        }

        public void VisitSynonym(Compiler.Functors.FunctorSynonym f)
        {
            Result = new Functors.FunctorSynonym(f.Identifier);
        }
    }
}
