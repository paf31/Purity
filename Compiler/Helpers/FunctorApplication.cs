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
    public class FunctorApplication : IFunctorVisitor
    {
        private readonly IType t;

        public IType Result { get; private set; }

        public FunctorApplication(IType t)
        {
            this.t = t;
        }

        public static IType Map(IFunctor f, IType t)
        {
            var visitor = new FunctorApplication(t);
            f.AcceptVisitor(visitor);
            return visitor.Result;
        }

        public void VisitArrow(Functors.ArrowFunctor f)
        {
            Result = new Types.ArrowType(f.Left, Map(f, t));
        }

        public void VisitConstant(Functors.ConstantFunctor f)
        {
            Result = f.Value;
        }

        public void VisitSynonym(Functors.FunctorSynonym f)
        {
            Result = Map(Container.ResolveFunctor(f.Identifier), t);
        }

        public void VisitIdentity(Functors.IdentityFunctor f)
        {
            Result = t;
        }

        public void VisitProduct(Functors.ProductFunctor f)
        {
            Result = new Types.ProductType(Map(f.Left, t), Map(f.Right, t));
        }

        public void VisitSum(Functors.SumFunctor f)
        {
            Result = new Types.SumType(Map(f.Left, t), Map(f.Right, t));
        }
    }
}
