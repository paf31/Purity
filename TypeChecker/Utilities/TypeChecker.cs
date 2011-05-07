using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Modules;
using Purity.Compiler.Typechecker.Helpers;
using Purity.Compiler.Typechecker.Classes;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Utilities
{
    public class TypeChecker
    {
        private readonly IMetadataContainer container;

        public TypeChecker(IMetadataContainer container) 
        {
            this.container = container;
        }

        public Tuple<IType, ITypedExpression> CreateTypedExpression(DataDeclaration decl)
        {
            TypeCheckingResult result = ConstraintCreator.W(decl.Data, container);

            IEnumerable<Constraint> constraints = result.Constraints;

            if (decl.Type != null)
            {
                var converted = PartialTypeCreator.Convert(decl.Type, null);
                var substitution = Unification.Unify(result.Type, converted);
                constraints = constraints.Concat(substitution);
            }

            SolutionSet solutionSet = constraints.Solve(result.MaximalIndex);

            if (decl.Type == null)
            {
                int maximalIndex = result.MaximalIndex;

                solutionSet.ReplaceUnknownsWithTypeParameters(maximalIndex);
            }

            IPartialType type = ApplySolutionSet.Apply(solutionSet, result.Type);
            IConstrainedData data = ApplySolutionSet.Apply(solutionSet, result.Data);

            return Tuple.Create(TypeConverter.Convert(type), DataConverter.Convert(data));
        }
    }
}
