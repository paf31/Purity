using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Typechecker.Helpers;
using Purity.Compiler.Typechecker.Types;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Typechecker.Utilities
{
    public static class TableauUtilities
    {
        public static bool SetTypeInTableau(Tableau tableau, int index, IPartialType replacement)
        {
            var merged = MergeTypesVisitor.Merge(tableau.Types[index], replacement, tableau);

            bool changed = merged != null;

            if (changed)
            {
                tableau.Types[index] = merged;
            }

            foreach (var type in tableau.Types.ToList())
            {
                changed |= TypeReplacer.Replace(type.Value, tableau, index, tableau.Types[index]);
            }

            return changed;
        }

        public static IType ReplaceUnknownsWithTypeVariables(Tableau tableau, int rootIndex)
        {
            IDictionary<int, int> representatives = CalculateRepresentatives(tableau);

            var collector = new UnknownTypeCollector();
            tableau.Types[rootIndex].AcceptVisitor(collector);

            int index = 1;

            IDictionary<int, TypeParameter> typeParameters = new Dictionary<int, TypeParameter>();

            foreach (var unknown in collector.Unknowns)
            {
                if (!typeParameters.ContainsKey(representatives[unknown]))
                {
                    typeParameters[representatives[unknown]] = new TypeParameter("T" + index++);
                }
            }

            foreach (var parameterIndex in typeParameters.Keys)
            {
                foreach (var unknown in representatives.Where(p => p.Value == parameterIndex).Select(p => p.Key))
                {
                    SetTypeInTableau(tableau, unknown, typeParameters[parameterIndex]);
                }
            }

            return TypeConverter.Convert(tableau.Types[rootIndex]);
        }

        public static IDictionary<int, int> CalculateRepresentatives(Tableau tableau)
        {
            IDictionary<int, int> classes = new Dictionary<int, int>();

            for (int i = 0; i < tableau.Types.Count; i++)
            {
                Mark(classes, i, i, tableau.Collisions);
            }

            return classes;
        }

        private static void Mark(IDictionary<int, int> classes, int index, int value, IList<Tuple<int, int>> edges)
        {
            if (!classes.ContainsKey(index))
            {
                classes[index] = value;

                foreach (var edge in edges)
                {
                    if (index == edge.Item1)
                    {
                        Mark(classes, edge.Item2, value, edges);
                    }
                    if (index == edge.Item2)
                    {
                        Mark(classes, edge.Item1, value, edges);
                    }
                }
            }
        }
    }
}
