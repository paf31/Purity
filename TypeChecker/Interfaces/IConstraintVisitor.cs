using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Typechecker.Interfaces
{
    public interface IConstraintVisitor
    {
        void VisitArrow(Constraints.ArrowConstraint c);

        void VisitFunctorApp(Constraints.FunctorAppConstraint c);

        void VisitFix(Constraints.FixConstraint c);

        void VisitLFix(Constraints.LFixConstraint c);

        void VisitGFix(Constraints.GFixConstraint c);
        
        void VisitProduct(Constraints.ProductConstraint c);
    
        void VisitSum(Constraints.SumConstraint c);
    }
}
