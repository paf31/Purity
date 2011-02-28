using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Typechecker.Types;
using Purity.Compiler.Typechecker.Functors;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class FunctorReplacer : IPartialTypeVisitor, IPartialFunctorVisitor
    {
        private readonly int index;
        private readonly IPartialFunctor replacement;

        public bool HasChanges
        {
            get;
            set;
        }

        public FunctorReplacer(int index, IPartialFunctor replacement)
        {
            this.index = index;
            this.replacement = replacement;
            this.HasChanges = false;
        }

        public void VisitArrow(Types.ArrowType t)
        {
            t.Left.AcceptVisitor(this);
            t.Right.AcceptVisitor(this);
        }

        public void VisitSynonym(Types.TypeSynonym t)
        {
            throw new CompilerException("Unexpected type synonym.");
        }

        public void VisitProduct(Types.ProductType t)
        {
            t.Left.AcceptVisitor(this);
            t.Right.AcceptVisitor(this);
        }

        public void VisitSum(Types.SumType t)
        {
            t.Left.AcceptVisitor(this);
            t.Right.AcceptVisitor(this);
        }

        public void VisitLFix(Types.LFixType t)
        {
            if (t.Functor is UnknownFunctor && (t.Functor as UnknownFunctor).Index == index)
            {
                HasChanges = !(replacement is UnknownType);
                t.Functor = replacement;
            }

            t.Functor.AcceptVisitor(this);
        }

        public void VisitGFix(Types.GFixType t)
        {
            if (t.Functor is UnknownFunctor && (t.Functor as UnknownFunctor).Index == index)
            {
                HasChanges = !(replacement is UnknownType);
                t.Functor = replacement;
            }

            t.Functor.AcceptVisitor(this);
        }

        public void VisitFunctorApp(Types.FunctorAppType t)
        {
            if (t.Functor is UnknownFunctor && (t.Functor as UnknownFunctor).Index == index)
            {
                HasChanges = !(replacement is UnknownType);
                t.Functor = replacement;
            }

            t.Argument.AcceptVisitor(this);
            t.Functor.AcceptVisitor(this);
        }

        public void VisitUnknown(Types.UnknownType unknownType)
        {
        }

        public void VisitArrow(Functors.ArrowFunctor f)
        {
            if (f.Right is UnknownFunctor && (f.Right as UnknownFunctor).Index == index)
            {
                HasChanges = !(replacement is UnknownType);
                f.Right = replacement;
            }

            f.Left.AcceptVisitor(this);
            f.Right.AcceptVisitor(this);
        }

        public void VisitConstant(Functors.ConstantFunctor f)
        {
            f.Value.AcceptVisitor(this);
        }

        public void VisitIdentity(Functors.IdentityFunctor f)
        {
        }

        public void VisitProduct(Functors.ProductFunctor f)
        {
            if (f.Left is UnknownFunctor && (f.Left as UnknownFunctor).Index == index)
            {
                HasChanges = !(replacement is UnknownType);
                f.Left = replacement;
            }

            if (f.Right is UnknownFunctor && (f.Right as UnknownFunctor).Index == index)
            {
                HasChanges = !(replacement is UnknownType);
                f.Right = replacement;
            }

            f.Left.AcceptVisitor(this);
            f.Right.AcceptVisitor(this);
        }

        public void VisitSum(Functors.SumFunctor f)
        {
            if (f.Left is UnknownFunctor && (f.Left as UnknownFunctor).Index == index)
            {
                HasChanges = !(replacement is UnknownType);
                f.Left = replacement;
            }

            if (f.Right is UnknownFunctor && (f.Right as UnknownFunctor).Index == index)
            {
                HasChanges = !(replacement is UnknownType);
                f.Right = replacement;
            }

            f.Left.AcceptVisitor(this);
            f.Right.AcceptVisitor(this);
        }

        public void VisitUnknown(Functors.UnknownFunctor unknownType)
        {
        }
    }
}
