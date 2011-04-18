using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Typechecker.Interfaces
{
    public interface IConstraintVisitor
    {
        void VisitArrow(Constraints.ArrowConstraint c);

        void VisitProduct(Constraints.ProductConstraint c);
    
        void VisitSum(Constraints.SumConstraint c);
    }

    public interface IConstraintVisitor<R>
    {
        R VisitArrow(Constraints.ArrowConstraint c);

        R VisitProduct(Constraints.ProductConstraint c);

        R VisitSum(Constraints.SumConstraint c);
    }
}
