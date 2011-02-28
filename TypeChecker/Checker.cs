using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Modules;
using Purity.Compiler.Extensions;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Helpers;

namespace Purity.Compiler.Typechecker
{
    public class Checker
    {
        public Checker(DataDeclaration decl)
        {
            Decl = decl;
        }

        public DataDeclaration Decl
        {
            get;
            set;
        }

        public Tableau Tableau
        {
            get;
            set;
        }

        public ITypedExpression CreateTypedExpression()
        {
            int rootIndex;
            var visitor = new ConstrainedDataCreator();
            visitor.Convert(Decl.Data, out rootIndex);

            Tableau = new Tableau(visitor.TypeIndex, visitor.FunctorIndex);
            Tableau.Types[rootIndex] = new PartialTypeCreator().Convert(Decl.Type);

            foreach (var type in visitor.KnownTypes)
            {
                if (type.Value != null)
                {
                    Tableau.Types[type.Key] = new PartialTypeCreator().Convert(type.Value.RemoveSynonyms());
                }
            }

            var enforcer = new ConstraintEnforcer(Tableau);

            while (true)
            {
                enforcer.HasChanges = false;

                foreach (var c in visitor.Constraints)
                {
                    c.AcceptVisitor(enforcer);
                }

                if (!enforcer.HasChanges)
                {
                    break;
                }
            }

            var merged = new TableauMerger(Tableau).Merge(visitor.Result);

            return new TypedExpressionCreator().Convert(merged);
        }
    }
}