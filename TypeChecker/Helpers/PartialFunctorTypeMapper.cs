using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Typechecker.Types;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Functors;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class PartialFunctorTypeMapper : IPartialFunctorVisitor
    {
        private IPartialType type;

        public IPartialType Result
        {
            get;
            set;
        }

        public static IPartialType Map(IPartialType type, IPartialFunctor functor)
        {
            var visitor = new PartialFunctorTypeMapper(type);
            functor.AcceptVisitor(visitor);
            return visitor.Result;
        }

        public PartialFunctorTypeMapper(IPartialType type)
        {
            this.type = type;
        }

        public void VisitArrow(Functors.ArrowFunctor f)
        {
            Result = new ArrowType(f.Left, Map(type, f.Right));
        }

        public void VisitConstant(Functors.ConstantFunctor f)
        {
            Result = f.Value;
        }

        public void VisitIdentity(Functors.IdentityFunctor f)
        {
            Result = type;
        }

        public void VisitProduct(Functors.ProductFunctor f)
        {
            Result = new ProductType(Map(type, f.Left), Map(type, f.Right));
        }

        public void VisitSum(Functors.SumFunctor f)
        {
            Result = new SumType(Map(type, f.Left), Map(type, f.Right));
        }

        public void VisitSynonym(Functors.FunctorSynonym f)
        {
            new PartialFunctorCreator().Convert(Container.ResolveFunctor(f.Identifier)).AcceptVisitor(this);
        }

        public void VisitUnknown(Functors.UnknownFunctor f)
        {
            throw new CompilerException(ErrorMessages.UnableToInferFunctor);
        }
    }
}
