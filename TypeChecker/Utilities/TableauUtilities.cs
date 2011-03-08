using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Typechecker.Helpers;
using Purity.Compiler.Typechecker.Types;
using Purity.Compiler.Typechecker.Functors;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Typechecker.Utilities
{
    public static class TableauUtilities
    {
        public static bool SetTypeInTableau(Tableau tableau, int index, IPartialType replacement)
        {
            bool changed;

            if (replacement == null)
            {
                throw new Exception();
            }

            tableau.Types[index] = MergeTypesVisitor.Merge(tableau.Types[index], replacement, out changed);

            foreach (var type in tableau.Types.ToList())
            {
                if (type.Value is UnknownType && (type.Value as UnknownType).Index == index)
                {
                    changed = !(tableau.Types[index] is UnknownType);
                    tableau.Types[type.Key] = tableau.Types[index];
                }
            }

            foreach (var type in tableau.Types.ToList())
            {
                var visitor = new TypeReplacer(index, tableau.Types[index]);
                type.Value.AcceptVisitor(visitor);
                changed |= visitor.HasChanges;
            }

            foreach (var application in tableau.FunctorApplications.Where(a => a.Value.Item2 == index))
            {
                var functor = tableau.Functors[application.Value.Item1];
                
                if (!(functor is UnknownFunctor))
                {
                    var applied = PartialFunctorTypeMapper.Map(tableau.Types[application.Value.Item2], tableau.Functors[application.Value.Item1]);
                    changed |= SetTypeInTableau(tableau, application.Key, applied);
                }
            }

            return changed;
        }

        public static bool SetFunctorInTableau(Tableau tableau, int index, IPartialFunctor replacement)
        {
            bool changed;

            tableau.Functors[index] = MergeFunctorsVisitor.Merge(tableau.Functors[index], replacement, out changed);

            foreach (var functor in tableau.Functors.ToList())
            {
                if (functor.Value is UnknownFunctor && (functor.Value as UnknownFunctor).Index == index)
                {
                    changed = !(tableau.Functors[index] is UnknownFunctor);
                    tableau.Functors[functor.Key] = tableau.Functors[index];
                }
            }

            foreach (var functor in tableau.Functors.ToList())
            {
                var visitor = new FunctorReplacer(index, tableau.Functors[index]);
                functor.Value.AcceptVisitor(visitor);
                changed |= visitor.HasChanges;
            }

            foreach (var application in tableau.FunctorApplications.Where(a => a.Value.Item1 == index))
            {
                var functor = tableau.Functors[application.Value.Item1];
                
                if (!(functor is UnknownFunctor))
                {
                    var applied = PartialFunctorTypeMapper.Map(tableau.Types[application.Value.Item2], tableau.Functors[application.Value.Item1]);
                    changed |= SetTypeInTableau(tableau, application.Key, applied);
                }
            }

            return changed;
        }
    }
}
