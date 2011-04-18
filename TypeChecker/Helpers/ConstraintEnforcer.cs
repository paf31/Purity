using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Typechecker.Types;
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
    }
}
