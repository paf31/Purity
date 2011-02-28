using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Typechecker.Types;
using Purity.Compiler.Typechecker.Functors;
using Purity.Compiler.Typechecker.Utilities;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class ConstraintEnforcer : IConstraintVisitor
    {
        private readonly Tableau tableau;

        public ConstraintEnforcer(Tableau tableau)
        {
            this.tableau = tableau;
        }

        public bool HasChanges
        {
            get;
            set;
        }

        public void VisitArrow(Constraints.ArrowConstraint c)
        {
            HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Index, new ArrowType(tableau.Types[c.Left], tableau.Types[c.Right]));

            if (tableau.Types[c.Index] is ArrowType)
            {
                HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Left, (tableau.Types[c.Index] as ArrowType).Left);
                HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Right, (tableau.Types[c.Index] as ArrowType).Right);
            }
        }

        public void VisitFunctorApp(Constraints.FunctorAppConstraint c)
        {
            HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Index, new FunctorAppType(tableau.Functors[c.Functor], tableau.Types[c.Argument]));

            if (tableau.Types[c.Index] is FunctorAppType)
            {
                HasChanges |= TableauUtilities.SetFunctorInTableau(tableau, c.Functor, (tableau.Types[c.Index] as FunctorAppType).Functor);
                HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Argument, (tableau.Types[c.Index] as FunctorAppType).Argument);
            }
        }

        public void VisitFix(Constraints.FixConstraint c)
        {
            if (tableau.Types[c.Index] is LFixType)
            {
                HasChanges |= TableauUtilities.SetFunctorInTableau(tableau, c.Functor, (tableau.Types[c.Index] as LFixType).Functor);
            }
            else if (tableau.Types[c.Index] is GFixType)
            {
                HasChanges |= TableauUtilities.SetFunctorInTableau(tableau, c.Functor, (tableau.Types[c.Index] as GFixType).Functor);
            }
        }

        public void VisitLFix(Constraints.LFixConstraint c)
        {
            HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Index, new LFixType(tableau.Functors[c.Functor]));

            if (tableau.Types[c.Index] is LFixType)
            {
                HasChanges |= TableauUtilities.SetFunctorInTableau(tableau, c.Functor, (tableau.Types[c.Index] as LFixType).Functor);
            }
        }

        public void VisitGFix(Constraints.GFixConstraint c)
        {
            HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Index, new GFixType(tableau.Functors[c.Functor]));

            if (tableau.Types[c.Index] is GFixType)
            {
                HasChanges |= TableauUtilities.SetFunctorInTableau(tableau, c.Functor, (tableau.Types[c.Index] as GFixType).Functor);
            }
        }

        public void VisitProduct(Constraints.ProductConstraint c)
        {
            HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Index, new ProductType(tableau.Types[c.Left], tableau.Types[c.Right]));

            if (tableau.Types[c.Index] is ProductType)
            {
                HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Left, (tableau.Types[c.Index] as ProductType).Left);
                HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Right, (tableau.Types[c.Index] as ProductType).Right);
            }
        }

        public void VisitSum(Constraints.SumConstraint c)
        {
            HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Index, new SumType(tableau.Types[c.Left], tableau.Types[c.Right]));

            if (tableau.Types[c.Index] is SumType)
            {
                HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Left, (tableau.Types[c.Index] as SumType).Left);
                HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Right, (tableau.Types[c.Index] as SumType).Right);
            }
        }
    }
}
