using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class MergeFunctorsVisitor : IPartialFunctorVisitor<IPartialFunctor>
    {
        private readonly Tableau tableau;
        private readonly IPartialFunctor replacement;

        public MergeFunctorsVisitor(IPartialFunctor replacement, Tableau tableau)
        {
            this.replacement = replacement;
            this.tableau = tableau;
        }

        public static IPartialFunctor Merge(IPartialFunctor f1, IPartialFunctor f2, Tableau tableau)
        {
            var visitor = new MergeFunctorsVisitor(f2, tableau);
            return f1.AcceptVisitor(visitor);
        }

        public IPartialFunctor VisitArrow(Functors.ArrowFunctor f)
        {
            if (replacement is Functors.UnknownFunctor)
            {
                return null;
            }
            else if (replacement is Functors.ArrowFunctor)
            {
                var f1 = replacement as Functors.ArrowFunctor;
                var left = MergeTypesVisitor.Merge(f.Left, f1.Left, tableau);
                var right = Merge(f.Right, f1.Right, tableau);
                return left == null && right == null ? null : new Functors.ArrowFunctor(left ?? f.Left, right ?? f.Right);
            }
            else
            {
                throw new CompilerException(ErrorMessages.ExpectedArrowFunctor);
            }
        }

        public IPartialFunctor VisitConstant(Functors.ConstantFunctor f)
        {
            if (replacement is Functors.UnknownFunctor)
            {
                return null;
            }
            else if (replacement is Functors.ConstantFunctor)
            {
                var f1 = replacement as Functors.ConstantFunctor;
                var value = MergeTypesVisitor.Merge(f.Value, f1.Value, tableau);
                return value == null ? null : new Functors.ConstantFunctor(value);
            }
            else
            {
                throw new CompilerException(ErrorMessages.ExpectedConstantFunctor);
            }
        }

        public IPartialFunctor VisitIdentity(Functors.IdentityFunctor f)
        {
            if (replacement is Functors.UnknownFunctor)
            {
                return null;
            }
            else if (replacement is Functors.IdentityFunctor)
            {
                return null;
            }
            else
            {
                throw new CompilerException(ErrorMessages.ExpectedIdentityFunctor);
            }
        }

        public IPartialFunctor VisitProduct(Functors.ProductFunctor f)
        {
            if (replacement is Functors.UnknownFunctor)
            {
                return null;
            }
            else if (replacement is Functors.ProductFunctor)
            {
                var f1 = replacement as Functors.ProductFunctor;
                var left = Merge(f.Left, f1.Left, tableau);
                var right = Merge(f.Right, f1.Right, tableau);
                return left == null && right == null ? null : new Functors.ProductFunctor(left ?? f.Left, right ?? f.Right);
            }
            else
            {
                throw new CompilerException(ErrorMessages.ExpectedProductFunctor);
            }
        }

        public IPartialFunctor VisitSum(Functors.SumFunctor f)
        {
            if (replacement is Functors.UnknownFunctor)
            {
                return null;
            }
            else if (replacement is Functors.SumFunctor)
            {
                var f1 = replacement as Functors.SumFunctor;
                var left = Merge(f.Left, f1.Left, tableau);
                var right = Merge(f.Right, f1.Right, tableau);
                return left == null && right == null ? null : new Functors.SumFunctor(left ?? f.Left, right ?? f.Right);
            }
            else
            {
                throw new CompilerException(ErrorMessages.ExpectedSumFunctor);
            }
        }

        public IPartialFunctor VisitUnknown(Functors.UnknownFunctor f)
        {
            if (replacement is Functors.UnknownFunctor)
            {
                return null;
            }
            else
            {
                return replacement;
            }
        }

        public IPartialFunctor VisitSynonym(Functors.FunctorSynonym f)
        {
            if (replacement is Functors.UnknownFunctor)
            {
                return null;
            }
            else if (replacement is Functors.FunctorSynonym)
            {
                if (f.Identifier.Equals((replacement as Functors.FunctorSynonym).Identifier))
                {
                    return null;
                }
                else
                {
                    throw new CompilerException(string.Format(ErrorMessages.Expected, f.Identifier));
                }
            }
            else
            {
                throw new CompilerException(string.Format(ErrorMessages.Expected, f.Identifier));
            }
        }
    }
}
