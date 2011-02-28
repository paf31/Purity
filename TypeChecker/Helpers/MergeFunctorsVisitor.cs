using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class MergeFunctorsVisitor : IPartialFunctorVisitor
    {
        private readonly IPartialFunctor replacement;

        public IPartialFunctor Result
        {
            get;
            set;
        }

        public bool Changed
        {
            get;
            set;
        }

        public MergeFunctorsVisitor(IPartialFunctor replacement)
        {
            this.replacement = replacement;
            this.Changed = false;
        }

        public static IPartialFunctor Merge(IPartialFunctor f1, IPartialFunctor f2, out bool changed)
        {
            var visitor = new MergeFunctorsVisitor(f2);
            f1.AcceptVisitor(visitor);
            changed = visitor.Changed;
            return visitor.Result;
        }

        public void VisitArrow(Functors.ArrowFunctor f)
        {
            if (replacement is Functors.UnknownFunctor)
            {
                Result = f;
                Changed = false;
            }
            else if (replacement is Functors.ArrowFunctor)
            {
                bool changed1, changed2;
                var f1 = replacement as Functors.ArrowFunctor;
                Result = new Functors.ArrowFunctor(MergeTypesVisitor.Merge(f.Left, f1.Left, out changed1), Merge(f.Right, f1.Right, out changed2));
                Changed = changed1 || changed2;
            }
            else
            {
                throw new CompilerException("Type error: expected arrow functor, found " + replacement.GetType().Name);
            }
        }

        public void VisitConstant(Functors.ConstantFunctor f)
        {
            if (replacement is Functors.UnknownFunctor)
            {
                Result = f;
            }
            else if (replacement is Functors.ConstantFunctor)
            {
                bool changed;
                var f1 = replacement as Functors.ConstantFunctor;
                Result = new Functors.ConstantFunctor(MergeTypesVisitor.Merge(f.Value, f1.Value, out changed));
                Changed = changed;
            }
            else
            {
                throw new CompilerException("Type error: expected constant functor, found " + replacement.GetType().Name);
            }
        }

        public void VisitIdentity(Functors.IdentityFunctor f)
        {
            if (replacement is Functors.UnknownFunctor)
            {
                Result = f;
            }
            else if (replacement is Functors.IdentityFunctor)
            {
                Result = new Functors.IdentityFunctor();
            }
            else
            {
                throw new CompilerException("Type error: expected identity functor, found " + replacement.GetType().Name);
            }
        }

        public void VisitProduct(Functors.ProductFunctor f)
        {
            if (replacement is Functors.UnknownFunctor)
            {
                Result = f;
            }
            else if (replacement is Functors.ProductFunctor)
            {
                bool changed1, changed2;
                var f1 = replacement as Functors.ProductFunctor;
                Result = new Functors.ProductFunctor(Merge(f.Left, f1.Left, out changed1), Merge(f.Right, f1.Right, out changed2));
                Changed = changed1 || changed2;
            }
            else
            {
                throw new CompilerException("Type error: expected product functor, found " + replacement.GetType().Name);
            }
        }

        public void VisitSum(Functors.SumFunctor f)
        {
            if (replacement is Functors.UnknownFunctor)
            {
                Result = f;
            }
            else if (replacement is Functors.SumFunctor)
            {
                bool changed1, changed2;
                var f1 = replacement as Functors.SumFunctor;
                Result = new Functors.SumFunctor(Merge(f.Left, f1.Left, out changed1), Merge(f.Right, f1.Right, out changed2));
                Changed = changed1 || changed2;
            }
            else
            {
                throw new CompilerException("Type error: expected sum functor, found " + replacement.GetType().Name);
            }
        }

        public void VisitUnknown(Functors.UnknownFunctor unknownType)
        {
            Result = replacement;

            if (!(replacement is Functors.UnknownFunctor))
            {
                Changed = true;
            }
        }
    }
}
