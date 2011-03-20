using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Functors;

namespace Purity.Compiler.Interfaces
{
    public interface IFunctorVisitor
    {
        void VisitArrow(ArrowFunctor f);

        void VisitConstant(ConstantFunctor f);

        void VisitSynonym(FunctorSynonym f);

        void VisitIdentity(IdentityFunctor f);

        void VisitProduct(ProductFunctor f);

        void VisitSum(SumFunctor f);
    }

    public interface IFunctorVisitor<R>
    {
        R VisitArrow(ArrowFunctor f);

        R VisitConstant(ConstantFunctor f);

        R VisitSynonym(FunctorSynonym f);

        R VisitIdentity(IdentityFunctor f);

        R VisitProduct(ProductFunctor f);

        R VisitSum(SumFunctor f);
    }
}
