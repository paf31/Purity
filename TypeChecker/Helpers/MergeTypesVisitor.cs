using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Extensions;
using Purity.Compiler.Typechecker.Functors;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class MergeTypesVisitor : IPartialTypeVisitor
    {
        private IPartialType replacement;

        public IPartialType Result
        {
            get;
            set;
        }

        public bool Changed
        {
            get;
            set;
        }

        public MergeTypesVisitor(IPartialType replacement)
        {
            this.replacement = replacement;
            this.Changed = false;
        }

        private void MergeFunctorAppType<T>(IPartialType t, IPartialFunctor functor) where T : IPartialFunctor
        {
            if (functor is UnknownFunctor)
            {
                Result = t;
                Changed = false;
            }
            else if (functor is T)
            {
                bool changed;
                var argument = (replacement as Types.FunctorAppType).Argument;
                var mapped = PartialFunctorTypeMapper.Map(argument, functor);
                Result = Merge(t, mapped, out changed);
                Changed = changed;
            }
            else
            {
                throw new CompilerException("Type error: expected functor application type, found " + replacement.GetType().Name);
            }
        }

        public void VisitArrow(Types.ArrowType t)
        {
            if (replacement is Types.FunctorAppType)
            {
                MergeFunctorAppType<ArrowFunctor>(t, (replacement as Types.FunctorAppType).Functor);
            }
            else if (replacement is Types.UnknownType)
            {
                Result = t;
                Changed = false;
            }
            else if (replacement is Types.ArrowType)
            {
                bool changed1, changed2;
                var t1 = replacement as Types.ArrowType;
                Result = new Types.ArrowType(Merge(t.Left, t1.Left, out changed1), Merge(t.Right, t1.Right, out changed2));
                Changed = changed1 || changed2;
            }
            else
            {
                throw new CompilerException("Type error: expected arrow type, found " + replacement.GetType().Name);
            }
        }

        public void VisitSynonym(Types.TypeSynonym t)
        {
            throw new CompilerException("Unexpected type synonym.");
        }

        public void VisitProduct(Types.ProductType t)
        {
            if (replacement is Types.FunctorAppType)
            {
                MergeFunctorAppType<ProductFunctor>(t, (replacement as Types.FunctorAppType).Functor);
            }
            else if (replacement is Types.UnknownType)
            {
                Result = t;
                Changed = false;
            }
            else if (replacement is Types.ProductType)
            {
                bool changed1, changed2;
                var t1 = replacement as Types.ProductType;
                Result = new Types.ProductType(Merge(t.Left, t1.Left, out changed1), Merge(t.Right, t1.Right, out changed2));
                Changed = changed1 || changed2;
            }
            else
            {
                throw new CompilerException("Type error: expected product type, found " + replacement.GetType().Name);
            }
        }

        public void VisitSum(Types.SumType t)
        {
            if (replacement is Types.FunctorAppType)
            {
                MergeFunctorAppType<SumFunctor>(t, (replacement as Types.FunctorAppType).Functor);
            }
            else if (replacement is Types.UnknownType)
            {
                Result = t;
                Changed = false;
            }
            else if (replacement is Types.SumType)
            {
                bool changed1, changed2;
                var t1 = replacement as Types.SumType;
                Result = new Types.SumType(Merge(t.Left, t1.Left, out changed1), Merge(t.Right, t1.Right, out changed2));
                Changed = changed1 || changed2;
            }
            else
            {
                throw new CompilerException("Type error: expected sum type, found " + replacement.GetType().Name);
            }
        }

        public void VisitLFix(Types.LFixType t)
        {
            if (replacement is Types.UnknownType)
            {
                Result = t;
                Changed = false;
            }
            else if (replacement is Types.LFixType)
            {
                bool changed;
                var t1 = replacement as Types.LFixType;
                Result = new Types.LFixType(MergeFunctorsVisitor.Merge(t.Functor, t1.Functor, out changed));
                Changed = changed;
            }
            else
            {
                throw new CompilerException("Type error: expected least fixed point type, found " + replacement.GetType().Name);
            }
        }

        public void VisitGFix(Types.GFixType t)
        {
            if (replacement is Types.UnknownType)
            {
                Result = t;
                Changed = false;
            }
            else if (replacement is Types.GFixType)
            {
                bool changed;
                var t1 = replacement as Types.GFixType;
                Result = new Types.GFixType(MergeFunctorsVisitor.Merge(t.Functor, t1.Functor, out changed));
                Changed = changed;
            }
            else
            {
                throw new CompilerException("Type error: expected greatest fixed point type, found " + replacement.GetType().Name);
            }
        }

        public void VisitFunctorApp(Types.FunctorAppType t)
        {
            if (replacement is Types.FunctorAppType)
            {
                bool changed1, changed2;
                var t1 = replacement as Types.FunctorAppType;
                Result = new Types.FunctorAppType(MergeFunctorsVisitor.Merge(t.Functor, t1.Functor, out changed1), Merge(t.Argument, t1.Argument, out changed2));
                Changed = changed1 || changed2;
            }
            else
            {
                Result = t;
                Changed = false;
            }
        }

        public void VisitUnknown(Types.UnknownType unknownType)
        {
            Result = replacement;

            if (!(replacement is Types.UnknownType))
            {
                Changed = true;
            }
        }

        public static IPartialType Merge(IPartialType t1, IPartialType t2, out bool changed)
        {
            var visitor = new MergeTypesVisitor(t2);
            t1.AcceptVisitor(visitor);
            changed = visitor.Changed;
            return visitor.Result;
        }
    }
}
