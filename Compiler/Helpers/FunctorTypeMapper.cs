using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Extensions;
using Purity.Core;
using Purity.Core.Types;

namespace Purity.Compiler.Helpers
{
    public class FunctorTypeMapper : IFunctorVisitor<Type>
    {
        private readonly Type t;
        private readonly Type[] genericParameters;

        public FunctorTypeMapper(Type t, Type[] genericParameters)
        {
            this.t = t;
            this.genericParameters = genericParameters;
        }

        public Type Map(IFunctor f)
        {
            return f.AcceptVisitor(this);
        }

        public Type VisitArrow(Functors.ArrowFunctor f)
        {
            var left = new TypeConverter(genericParameters).Convert(f.Left);
            return typeof(IFunction<,>).MakeGenericType(left, Map(f.Right));
        }

        public Type VisitConstant(Functors.ConstantFunctor f)
        {
            return new TypeConverter(genericParameters).Convert(f.Value);
        }

        public Type VisitSynonym(Functors.FunctorSynonym f)
        {
            return Map(Container.ResolveFunctor(f.Identifier).Functor);
        }

        public Type VisitIdentity(Functors.IdentityFunctor f)
        {
            return t;
        }

        public Type VisitProduct(Functors.ProductFunctor f)
        {
            return typeof(Tuple<,>).MakeGenericType(Map(f.Left), Map(f.Right));
        }

        public Type VisitSum(Functors.SumFunctor f)
        {
            return typeof(Either<,>).MakeGenericType(Map(f.Left), Map(f.Right));
        }
    }
}
