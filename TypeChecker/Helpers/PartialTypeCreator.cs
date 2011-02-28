﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class PartialTypeCreator : ITypeVisitor
    {
        public IPartialType Result
        {
            get;
            set;
        }

        public IPartialType Convert(IType type)
        {
            type.AcceptVisitor(this);
            return Result;
        }

        public void VisitArrow(Compiler.Types.ArrowType t)
        {
            Result = new Types.ArrowType(Convert(t.Left), Convert(t.Right));
        }

        public void VisitSynonym(Compiler.Types.TypeSynonym t)
        {
            throw new CompilerException("Unexpected type synonym.");
        }

        public void VisitProduct(Compiler.Types.ProductType t)
        {
            Result = new Types.ProductType(Convert(t.Left), Convert(t.Right));
        }

        public void VisitSum(Compiler.Types.SumType t)
        {
            Result = new Types.SumType(Convert(t.Left), Convert(t.Right));
        }

        public void VisitLFix(Compiler.Types.LFixType t)
        {
            Result = new Types.LFixType(new PartialFunctorCreator().Convert(t.Functor));
        }

        public void VisitGFix(Compiler.Types.GFixType t)
        {
            Result = new Types.GFixType(new PartialFunctorCreator().Convert(t.Functor));
        }

        public void VisitFunctorApp(Compiler.Types.FunctorAppType t)
        {
            Result = new Types.FunctorAppType(new PartialFunctorCreator().Convert(t.Functor), Convert(t.Argument));
        }
    }
}
