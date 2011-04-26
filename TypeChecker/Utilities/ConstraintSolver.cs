using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Classes;
using Purity.Compiler.Typechecker.Helpers;

namespace Purity.Compiler.Typechecker.Utilities
{
    public static class ConstraintSolver
    {
        public static SolutionSet Solve(this IEnumerable<Constraint> constraints, int maximalIndex)
        {
            IList<Constraint> constraintsCopy = new List<Constraint>(constraints);

            SolutionSet solutionSet = new SolutionSet();

            int constraintIndex = 0;

            while (constraintIndex < constraintsCopy.Count)
            {
                Constraint constraint = constraintsCopy[constraintIndex];

                for (int i = 0; i < maximalIndex; i++)
                {
                    solutionSet[i] = TypeReplacer.Replace(solutionSet[i], constraint);
                }

                int constraintCount = constraintsCopy.Count;

                for (int i = constraintIndex + 1; i < constraintCount; i++)
                {
                    Constraint nextConstraint = constraintsCopy[i];

                    if (nextConstraint.Index == constraint.Index)
                    {
                        var newConstraints = Unification.Unify(constraint.Type, nextConstraint.Type);

                        foreach (Constraint newConstraint in newConstraints)
                        {
                            constraintsCopy.Add(newConstraint);
                        }
                    }
                    else
                    {
                        nextConstraint.Type = TypeReplacer.Replace(nextConstraint.Type, constraint);
                    }
                }

                constraintIndex++;
            }

            return solutionSet;
        }

        public static void ReplaceUnknownsWithTypeParameters(this SolutionSet solutionSet, int maximalIndex)
        {
            int parameterIndex = 1;
           
            for (int i= 0; i < maximalIndex; i++)
            {
                var unknown = solutionSet[i] as Types.UnknownType;
           
                if (unknown != null && unknown.Index == i) 
                {
                    var parameter = new Types.TypeParameter("T" + parameterIndex++);
                    var constraint = new Constraint(i, parameter);
           
                    for (int j = 0; j < maximalIndex; j++)
                    {
                        solutionSet[j] = TypeReplacer.Replace(solutionSet[j], constraint);
                    }
                }
            }
        }
    }
}
