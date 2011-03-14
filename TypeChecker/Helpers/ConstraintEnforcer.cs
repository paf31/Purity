using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Typechecker.Types;
using Purity.Compiler.Typechecker.Functors;
using Purity.Compiler.Typechecker.Utilities;
using Purity.Compiler.Exceptions;

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

            var current = tableau.Types[c.Index];

            if (current is ArrowType)
            {
                HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Left, (current as ArrowType).Left);
                HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Right, (current as ArrowType).Right);
            }
            else if (!(current is UnknownType)) 
            {
                throw new CompilerException(ErrorMessages.ExpectedArrowType); 
            }
        }

        public void VisitFix(Constraints.FixConstraint c)
        {
            var current = tableau.Types[c.Index];

            if (current is LFixType)
            {
                HasChanges |= TableauUtilities.SetFunctorInTableau(tableau, c.Functor, (current as LFixType).Functor);
            }
            else if (current is GFixType)
            {
                HasChanges |= TableauUtilities.SetFunctorInTableau(tableau, c.Functor, (current as GFixType).Functor);
            }
            else if (!(current is UnknownType))
            {
                throw new CompilerException(ErrorMessages.ExpectedFixedPointType);
            }
        }

        public void VisitLFix(Constraints.LFixConstraint c)
        {
            HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Index, new LFixType(tableau.Functors[c.Functor]));

            var current = tableau.Types[c.Index];

            if (current is LFixType)
            {
                HasChanges |= TableauUtilities.SetFunctorInTableau(tableau, c.Functor, (current as LFixType).Functor);
            }
            else if (!(current is UnknownType))
            {
                throw new CompilerException(ErrorMessages.ExpectedLFixType);
            }
        }

        public void VisitGFix(Constraints.GFixConstraint c)
        {
            HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Index, new GFixType(tableau.Functors[c.Functor]));

            var current = tableau.Types[c.Index];

            if (current is GFixType)
            {
                HasChanges |= TableauUtilities.SetFunctorInTableau(tableau, c.Functor, (current as GFixType).Functor);
            }
            else if (!(current is UnknownType))
            {
                throw new CompilerException(ErrorMessages.ExpectedGFixType);
            }
        }

        public void VisitProduct(Constraints.ProductConstraint c)
        {
            HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Index, new ProductType(tableau.Types[c.Left], tableau.Types[c.Right]));

            var current = tableau.Types[c.Index];

            if (current is ProductType)
            {
                HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Left, (current as ProductType).Left);
                HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Right, (current as ProductType).Right);
            }
            else if (!(current is UnknownType))
            {
                throw new CompilerException(ErrorMessages.ExpectedProductType);
            }
        }

        public void VisitSum(Constraints.SumConstraint c)
        {
            HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Index, new SumType(tableau.Types[c.Left], tableau.Types[c.Right]));

            var current = tableau.Types[c.Index];

            if (current is SumType)
            {
                HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Left, (current as SumType).Left);
                HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Right, (current as SumType).Right);
            }
            else if (!(current is UnknownType))
            {
                throw new CompilerException(ErrorMessages.ExpectedSumType);
            }
        }

        public void VisitSynonym(Constraints.SynonymConstraint c)
        {
            var current = tableau.Types[c.Index];

            if (current is TypeSynonym)
            {
                var resolved = Container.ResolveType((current as TypeSynonym).Identifier);
                var converted = new PartialTypeCreator().Convert(resolved);
                HasChanges |= TableauUtilities.SetTypeInTableau(tableau, c.Target, converted);
            }
            else if (!(current is UnknownType))
            {
                throw new CompilerException(ErrorMessages.ExpectedTypeSynonym);
            }
        }
    }
}
