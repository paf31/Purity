using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Typechecker.Types;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class TypeReplacer : IPartialTypeVisitor, IPartialFunctorVisitor
    {
        private readonly int index;
        private readonly IPartialType replacement;

        public bool HasChanges
        {
            get;
            set;
        }

        public TypeReplacer(int index, IPartialType replacement)
        {
            this.index = index;
            this.replacement = replacement;
            this.HasChanges = false;
        }

        public void VisitArrow(Types.ArrowType t)
        {
            if (t.Left is UnknownType && (t.Left as UnknownType).Index == index)
            {
                HasChanges = !(replacement is UnknownType);
                t.Left = replacement;
            }

            if (t.Right is UnknownType && (t.Right as UnknownType).Index == index)
            {
                HasChanges = !(replacement is UnknownType);
                t.Right = replacement;
            }

            t.Left.AcceptVisitor(this);
            t.Right.AcceptVisitor(this);
        }

        public void VisitSynonym(Types.TypeSynonym t)
        {
            throw new CompilerException("Unexpected type synonym.");
        }

        public void VisitProduct(Types.ProductType t)
        {
            if (t.Left is UnknownType && (t.Left as UnknownType).Index == index)
            {
                HasChanges = !(replacement is UnknownType);
                t.Left = replacement;
            }

            if (t.Right is UnknownType && (t.Right as UnknownType).Index == index)
            {
                HasChanges = !(replacement is UnknownType);
                t.Right = replacement;
            }

            t.Left.AcceptVisitor(this);
            t.Right.AcceptVisitor(this);
        }

        public void VisitSum(Types.SumType t)
        {
            if (t.Left is UnknownType && (t.Left as UnknownType).Index == index)
            {
                HasChanges = !(replacement is UnknownType);
                t.Left = replacement;
            }

            if (t.Right is UnknownType && (t.Right as UnknownType).Index == index)
            {
                HasChanges = !(replacement is UnknownType);
                t.Right = replacement;
            }

            t.Left.AcceptVisitor(this);
            t.Right.AcceptVisitor(this);
        }

        public void VisitLFix(Types.LFixType t)
        {
            t.Functor.AcceptVisitor(this);
        }

        public void VisitGFix(Types.GFixType t)
        {
            t.Functor.AcceptVisitor(this);
        }

        public void VisitFunctorApp(Types.FunctorAppType t)
        {
            if (t.Argument is UnknownType && (t.Argument as UnknownType).Index == index)
            {
                HasChanges = !(replacement is UnknownType);
                t.Argument = replacement;
            }

            t.Argument.AcceptVisitor(this);
            t.Functor.AcceptVisitor(this);
        }

        public void VisitUnknown(Types.UnknownType unknownType)
        {
        }

        public void VisitArrow(Functors.ArrowFunctor f)
        {
            if (f.Left is UnknownType && (f.Left as UnknownType).Index == index)
            {
                HasChanges = !(replacement is UnknownType);
                f.Left = replacement;
            }

            f.Left.AcceptVisitor(this);
            f.Right.AcceptVisitor(this);
        }

        public void VisitConstant(Functors.ConstantFunctor f)
        {
            if (f.Value is UnknownType && (f.Value as UnknownType).Index == index)
            {
                HasChanges = !(replacement is UnknownType);
                f.Value = replacement;
            }

            f.Value.AcceptVisitor(this);
        }

        public void VisitIdentity(Functors.IdentityFunctor f)
        {
        }

        public void VisitProduct(Functors.ProductFunctor f)
        {
            f.Left.AcceptVisitor(this);
            f.Right.AcceptVisitor(this);
        }

        public void VisitSum(Functors.SumFunctor f)
        {
            f.Left.AcceptVisitor(this);
            f.Right.AcceptVisitor(this);
        }

        public void VisitUnknown(Functors.UnknownFunctor unknownType)
        {
        }
    }
}
