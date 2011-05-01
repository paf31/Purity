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
    public class FunctorApplication : IFunctorVisitor<IType>
    {
        private readonly IType t;

        public FunctorApplication(IType t)
        {
            this.t = t;
        }

        public static IType Map(IFunctor f, IType t)
        {
            var visitor = new FunctorApplication(t);
            return f.AcceptVisitor(visitor);
        }

        public IType VisitArrow(Functors.ArrowFunctor f)
        {
            return new Types.ArrowType(f.Left, Map(f.Right, t));
        }

        public IType VisitConstant(Functors.ConstantFunctor f)
        {
            return f.Value;
        }

        public IType VisitSynonym(Functors.FunctorSynonym f)
        {
            return Map(Container.ResolveFunctor(f.Identifier).Functor, t);
        }

        public IType VisitIdentity(Functors.IdentityFunctor f)
        {
            return t;
        }

        public IType VisitProduct(Functors.ProductFunctor f)
        {
            return new Types.ProductType(Map(f.Left, t), Map(f.Right, t));
        }

        public IType VisitSum(Functors.SumFunctor f)
        {
            return new Types.SumType(Map(f.Left, t), Map(f.Right, t));
        }
    }
}
