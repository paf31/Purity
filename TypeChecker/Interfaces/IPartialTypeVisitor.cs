using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Types;

namespace Purity.Compiler.Typechecker.Interfaces
{
    public interface IPartialTypeVisitor
    {
        void VisitArrow(ArrowType t);

        void VisitSynonym(TypeSynonym t);

        void VisitProduct(ProductType t);

        void VisitSum(SumType t);

        void VisitLFix(LFixType t);

        void VisitGFix(GFixType t);

        void VisitFunctorApp(FunctorAppType t);

        void VisitUnknown(UnknownType t);
    }
}
