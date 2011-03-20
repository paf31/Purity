using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Typechecker.Interfaces
{
    public interface IConstraintVisitor
    {
        void VisitArrow(Constraints.ArrowConstraint c);

        void VisitFix(Constraints.FixConstraint c);

        void VisitLFix(Constraints.LFixConstraint c);

        void VisitGFix(Constraints.GFixConstraint c);
        
        void VisitProduct(Constraints.ProductConstraint c);
    
        void VisitSum(Constraints.SumConstraint c);

        void VisitSynonym(Constraints.SynonymConstraint c);
    }

    public interface IConstraintVisitor<R>
    {
        R VisitArrow(Constraints.ArrowConstraint c);

        R VisitFix(Constraints.FixConstraint c);

        R VisitLFix(Constraints.LFixConstraint c);

        R VisitGFix(Constraints.GFixConstraint c);

        R VisitProduct(Constraints.ProductConstraint c);

        R VisitSum(Constraints.SumConstraint c);

        R VisitSynonym(Constraints.SynonymConstraint c);
    }
}
