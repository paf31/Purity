using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class FunctorConverter : IPartialFunctorVisitor<IFunctor>
    {
        public static IFunctor Convert(IPartialFunctor functor)
        {
            return functor.AcceptVisitor(new FunctorConverter());
        }

        public IFunctor VisitArrow(Functors.ArrowFunctor f)
        {
            return new Purity.Compiler.Functors.ArrowFunctor(TypeConverter.Convert(f.Left), Convert(f.Right));
        }

        public IFunctor VisitConstant(Functors.ConstantFunctor f)
        {
            return new Purity.Compiler.Functors.ConstantFunctor(TypeConverter.Convert(f.Value));
        }

        public IFunctor VisitIdentity(Functors.IdentityFunctor f)
        {
            return new Purity.Compiler.Functors.IdentityFunctor();
        }

        public IFunctor VisitProduct(Functors.ProductFunctor f)
        {
            return new Purity.Compiler.Functors.ProductFunctor(Convert(f.Left), Convert(f.Right));
        }

        public IFunctor VisitSum(Functors.SumFunctor f)
        {
            return new Purity.Compiler.Functors.SumFunctor(Convert(f.Left), Convert(f.Right));
        }

        public IFunctor VisitSynonym(Functors.FunctorSynonym f)
        {
            return new Purity.Compiler.Functors.FunctorSynonym(f.Identifier);
        }

        public IFunctor VisitUnknown(Functors.UnknownFunctor f)
        {
            throw new CompilerException(ErrorMessages.UnableToInferFunctor);
        }
    }
}