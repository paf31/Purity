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
    public class PartialFunctorTypeMapper : IPartialFunctorVisitor<IPartialType>
    {
        private readonly IPartialType type;

        public static IPartialType Map(IPartialType type, IPartialFunctor functor)
        {
            var visitor = new PartialFunctorTypeMapper(type);
            return functor.AcceptVisitor(visitor);
        }

        public PartialFunctorTypeMapper(IPartialType type)
        {
            this.type = type;
        }

        public IPartialType VisitArrow(Functors.ArrowFunctor f)
        {
            return new ArrowType(f.Left, Map(type, f.Right));
        }

        public IPartialType VisitConstant(Functors.ConstantFunctor f)
        {
            return f.Value;
        }

        public IPartialType VisitIdentity(Functors.IdentityFunctor f)
        {
            return type;
        }

        public IPartialType VisitProduct(Functors.ProductFunctor f)
        {
            return new ProductType(Map(type, f.Left), Map(type, f.Right));
        }

        public IPartialType VisitSum(Functors.SumFunctor f)
        {
            return new SumType(Map(type, f.Left), Map(type, f.Right));
        }

        public IPartialType VisitSynonym(Functors.FunctorSynonym f)
        {
            var resolved = Container.ResolveFunctor(f.Identifier);
            var converted = PartialFunctorCreator.Convert(resolved);
            return converted.AcceptVisitor(this);
        }

        public IPartialType VisitUnknown(Functors.UnknownFunctor f)
        {
            throw new CompilerException(ErrorMessages.UnableToInferFunctor);
        }
    }
}
