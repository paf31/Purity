using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Typechecker.Functors;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class MergeTypesVisitor : IPartialTypeVisitor<IPartialType>
    {
        private readonly Tableau tableau;
        private readonly IPartialType replacement;

        public MergeTypesVisitor(IPartialType replacement, Tableau tableau)
        {
            this.tableau = tableau;
            this.replacement = replacement;
        }

        public static IPartialType Merge(IPartialType t1, IPartialType t2, Tableau tableau)
        {
            var visitor = new MergeTypesVisitor(t2, tableau);
            return t1.AcceptVisitor(visitor);
        }

        public IPartialType VisitArrow(Types.ArrowType t)
        {
            if (replacement is Types.UnknownType)
            {
                return null;
            }
            else if (replacement is Types.ArrowType)
            {
                var t1 = replacement as Types.ArrowType;
                var left = Merge(t.Left, t1.Left, tableau);
                var right = Merge(t.Right, t1.Right, tableau);
                return left == null && right == null ? null : new Types.ArrowType(left ?? t.Left, right ?? t.Right);
            }
            else
            {
                throw new CompilerException(ErrorMessages.ExpectedArrowType);
            }
        }

        public IPartialType VisitSynonym(Types.TypeSynonym t)
        {
            if (replacement is Types.UnknownType)
            {
                return null;
            }
            else if (replacement is Types.TypeSynonym)
            {
                if (t.Identifier.Equals((replacement as Types.TypeSynonym).Identifier))
                {
                    return null;
                }
                else
                {
                    throw new CompilerException(string.Format(ErrorMessages.Expected, t.Identifier));
                }
            }
            else
            {
                throw new CompilerException(string.Format(ErrorMessages.Expected, t.Identifier));
            }
        }

        public IPartialType VisitProduct(Types.ProductType t)
        {
            if (replacement is Types.UnknownType)
            {
                return null;
            }
            else if (replacement is Types.ProductType)
            {
                var t1 = replacement as Types.ProductType;
                var left = Merge(t.Left, t1.Left, tableau);
                var right = Merge(t.Right, t1.Right, tableau);
                return left == null && right == null ? null : new Types.ProductType(left ?? t.Left, right ?? t.Right);
            }
            else
            {
                throw new CompilerException(ErrorMessages.ExpectedProductType);
            }
        }

        public IPartialType VisitSum(Types.SumType t)
        {
            if (replacement is Types.UnknownType)
            {
                return null;
            }
            else if (replacement is Types.SumType)
            {
                var t1 = replacement as Types.SumType;
                var left = Merge(t.Left, t1.Left, tableau);
                var right = Merge(t.Right, t1.Right, tableau);
                return left == null && right == null ? null : new Types.SumType(left ?? t.Left, right ?? t.Right);
            }
            else
            {
                throw new CompilerException(ErrorMessages.ExpectedSumType);
            }
        }

        public IPartialType VisitLFix(Types.LFixType t)
        {
            if (replacement is Types.UnknownType)
            {
                return null;
            }
            else if (replacement is Types.LFixType)
            {
                var t1 = replacement as Types.LFixType;
                var functor = MergeFunctorsVisitor.Merge(t.Functor, t1.Functor, tableau);
                if (functor == null)
                {
                    return null;
                }
                else
                {
                    var result = new Types.LFixType(functor);
                    result.Identifier = t.Identifier ?? t1.Identifier;
                    return string.Equals(result.Identifier, t.Identifier) ? null : result;
                }
            }
            else
            {
                throw new CompilerException(ErrorMessages.ExpectedLFixType);
            }
        }

        public IPartialType VisitGFix(Types.GFixType t)
        {
            if (replacement is Types.UnknownType)
            {
                return null;
            }
            else if (replacement is Types.GFixType)
            {
                var t1 = replacement as Types.GFixType;
                var functor = MergeFunctorsVisitor.Merge(t.Functor, t1.Functor, tableau);
                if (functor == null)
                {
                    return null;
                }
                else
                {
                    var result = new Types.GFixType(functor);
                    result.Identifier = t.Identifier ?? t1.Identifier;
                    return string.Equals(result.Identifier, t.Identifier) ? null : result;
                }
            }
            else
            {
                throw new CompilerException(ErrorMessages.ExpectedGFixType);
            }
        }

        public IPartialType VisitUnknown(Types.UnknownType t)
        {
            var unknown = replacement as Types.UnknownType;

            if (unknown != null)
            {
                if (unknown.Index != t.Index)
                {
                    tableau.AddCollision(unknown.Index, t.Index);
                }

                return null;
            }
            else
            {
                return replacement;
            }
        }

        public IPartialType VisitParameter(Types.TypeParameter t)
        {
            if (replacement is Types.UnknownType)
            {
                return null;
            }
            else if (replacement is Types.TypeParameter)
            {
                if (t.Identifier.Equals((replacement as Types.TypeParameter).Identifier))
                {
                    return null;
                }
                else
                {
                    throw new CompilerException(string.Format(ErrorMessages.Expected, t.Identifier));
                }
            }
            else
            {
                throw new CompilerException(ErrorMessages.ExpectedNamedType);
            }
        }
    }
}
