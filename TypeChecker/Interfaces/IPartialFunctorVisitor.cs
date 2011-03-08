using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Types;
using Purity.Compiler.Typechecker.Functors;

namespace Purity.Compiler.Typechecker.Interfaces
{
    public interface IPartialFunctorVisitor
    {
        void VisitArrow(ArrowFunctor f);

        void VisitConstant(ConstantFunctor f);

        void VisitIdentity(IdentityFunctor f);

        void VisitProduct(ProductFunctor f);

        void VisitSum(SumFunctor f);

        void VisitUnknown(UnknownFunctor f);

        void VisitSynonym(FunctorSynonym f);
    }
}
