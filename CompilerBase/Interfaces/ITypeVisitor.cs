using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Types;

namespace Purity.Compiler.Interfaces
{
    public interface ITypeVisitor
    {
        void VisitArrow(ArrowType t);

        void VisitSynonym(TypeSynonym t);

        void VisitProduct(ProductType t);

        void VisitSum(SumType t);

        void VisitLFix(LFixType t);

        void VisitGFix(GFixType t);
    }
}
