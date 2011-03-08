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
    public class FunctorTypeMapper : IFunctorVisitor
    {
        private readonly Type t;

        public Type Result { get; private set; }

        public FunctorTypeMapper(Type t)
        {
            this.t = t;
        }

        public Type Map(IFunctor f)
        {
            f.AcceptVisitor(this);
            return Result;
        }

        public void VisitArrow(Functors.ArrowFunctor f)
        {
            Result = typeof(IFunction<,>).MakeGenericType(new TypeConverter().Convert(f.Left), Map(f.Right));
        }

        public void VisitConstant(Functors.ConstantFunctor f)
        {
            Result = new TypeConverter().Convert(f.Value);
        }

        public void VisitSynonym(Functors.FunctorSynonym f)
        {
            Result = Map(Container.ResolveFunctor(f.Identifier));
        }

        public void VisitIdentity(Functors.IdentityFunctor f)
        {
            Result = t;
        }

        public void VisitProduct(Functors.ProductFunctor f)
        {
            Result = typeof(Tuple<,>).MakeGenericType(Map(f.Left), Map(f.Right));
        }

        public void VisitSum(Functors.SumFunctor f)
        {
            Result = typeof(Either<,>).MakeGenericType(Map(f.Left), Map(f.Right));
        }
    }
}
