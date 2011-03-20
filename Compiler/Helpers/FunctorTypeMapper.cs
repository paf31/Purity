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

        public FunctorTypeMapper(Type t)
        {
            this.t = t;
        }

        public Type Map(IFunctor f)
        {
            return f.AcceptVisitor(this);
        }

        public Type VisitArrow(Functors.ArrowFunctor f)
        {
            return typeof(IFunction<,>).MakeGenericType(new TypeConverter(null).Convert(f.Left), Map(f.Right));
        }

        public Type VisitConstant(Functors.ConstantFunctor f)
        {
            return new TypeConverter(null).Convert(f.Value);
        }

        public Type VisitSynonym(Functors.FunctorSynonym f)
        {
            return Map(Container.ResolveFunctor(f.Identifier));
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
