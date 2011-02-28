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
}
