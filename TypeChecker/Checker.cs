using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Modules;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Helpers;
using Purity.Compiler.Typechecker.Utilities;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker
{
    public static class Checker
    {
        public static ITypedExpression CreateTypedExpression(DataDeclaration decl)
        {
            int rootIndex;
            var visitor = new ConstrainedDataCreator();
            var constrainedData = visitor.Convert(decl.Data, out rootIndex);

            CycleDetector.ThrowOnCycleDetected(visitor.Constraints);

            Tableau tableau = new Tableau(visitor.TypeIndex, visitor.FunctorIndex);

            foreach (var type in visitor.KnownTypes)
            {
                if (type.Value != null)
                {
                    TableauUtilities.SetTypeInTableau(tableau, type.Key, type.Value);
                }
            }

            if (decl.Type != null)
            {
                TableauUtilities.SetTypeInTableau(tableau, rootIndex, PartialTypeCreator.Convert(decl.Type, null));
            }

            EnforceConstraints(tableau, visitor.Constraints);

            if (decl.Type == null)
            {
                decl.Type = TableauUtilities.ReplaceUnknownsWithTypeVariables(tableau, rootIndex);
            }

            var merged = TableauMerger.Merge(constrainedData, tableau);

            return TypedExpressionCreator.Convert(merged);
        }

        private static void EnforceConstraints(Tableau tableau, IEnumerable<IConstraint> constraints)
        {
            var enforcer = new ConstraintEnforcer(tableau);

            while (true)
            {
                enforcer.HasChanges = false;

                foreach (var c in constraints)
                {
                    c.AcceptVisitor(enforcer);
                }

                if (!enforcer.HasChanges)
                {
                    break;
                }
            }
        }
    }
}