﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Exceptions;
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

        public static IPartialType Merge(IPartialType t1, IPartialType t2, out bool changed)
        {
            var visitor = new MergeTypesVisitor(t2);
            t1.AcceptVisitor(visitor);
            changed = visitor.Changed;
            return visitor.Result;
        }

        public void VisitArrow(Types.ArrowType t)
        {
            if (replacement is Types.UnknownType)
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
                throw new CompilerException(ErrorMessages.ExpectedArrowType);
            }
        }

        public void VisitSynonym(Types.TypeSynonym t)
        {
            if (replacement is Types.UnknownType)
            {
                Result = t;
                Changed = false;
            }
            else if (replacement is Types.TypeSynonym)
            {
                if (t.Identifier.Equals((replacement as Types.TypeSynonym).Identifier))
                {
                    Result = t;
                    Changed = false;
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

        public void VisitProduct(Types.ProductType t)
        {
            if (replacement is Types.UnknownType)
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
                throw new CompilerException(ErrorMessages.ExpectedProductType);
            }
        }

        public void VisitSum(Types.SumType t)
        {
            if (replacement is Types.UnknownType)
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
                throw new CompilerException(ErrorMessages.ExpectedSumType);
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
                var result = new Types.LFixType(MergeFunctorsVisitor.Merge(t.Functor, t1.Functor, out changed));
                result.Identifier = t.Identifier ?? t1.Identifier;
                Result = result;
                Changed = changed;
            }
            else
            {
                throw new CompilerException(ErrorMessages.ExpectedLFixType);
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
                var result = new Types.GFixType(MergeFunctorsVisitor.Merge(t.Functor, t1.Functor, out changed));
                result.Identifier = t.Identifier ?? t1.Identifier;
                Result = result;
                Changed = changed;
            }
            else
            {
                throw new CompilerException(ErrorMessages.ExpectedGFixType);
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
    }
}
