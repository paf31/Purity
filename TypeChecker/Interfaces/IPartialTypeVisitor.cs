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

        void VisitUnknown(UnknownType t);

        void VisitParameter(TypeParameter t);
    }

    public interface IPartialTypeVisitor<R>
    {
        R VisitArrow(ArrowType t);

        R VisitSynonym(TypeSynonym t);

        R VisitProduct(ProductType t);

        R VisitSum(SumType t);

        R VisitUnknown(UnknownType t);

        R VisitParameter(TypeParameter t);
    }
}
