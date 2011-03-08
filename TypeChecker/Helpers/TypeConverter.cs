using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class TypeConverter : IPartialTypeVisitor
    {
        public IType Result
        {
            get;
            set;
        }

        public IType Convert(IPartialType type)
        {
            type.AcceptVisitor(this);
            return Result;
        }

        public void VisitArrow(Types.ArrowType t)
        {
            Result = new Purity.Compiler.Types.ArrowType(Convert(t.Left), Convert(t.Right));
        }

        public void VisitSynonym(Types.TypeSynonym t)
        {
            Result = new Purity.Compiler.Types.TypeSynonym(t.Identifier);
        }

        public void VisitProduct(Types.ProductType t)
        {
            Result = new Purity.Compiler.Types.ProductType(Convert(t.Left), Convert(t.Right));
        }

        public void VisitSum(Types.SumType t)
        {
            Result = new Purity.Compiler.Types.SumType(Convert(t.Left), Convert(t.Right));
        }

        public void VisitLFix(Types.LFixType t)
        {
            if (t.Identifier == null)
            {
                throw new CompilerException(ErrorMessages.UnableToInferFunctor);
            }

            var result = new Purity.Compiler.Types.LFixType(new FunctorConverter().Convert(t.Functor));
            result.Identifier = t.Identifier;
            Result = result;
        }

        public void VisitGFix(Types.GFixType t)
        {
            if (t.Identifier == null)
            {
                throw new CompilerException(ErrorMessages.UnableToInferFunctor);
            }

            var result = new Purity.Compiler.Types.GFixType(new FunctorConverter().Convert(t.Functor));
            result.Identifier = t.Identifier;
            Result = result;
        }

        public void VisitUnknown(Types.UnknownType unknownType)
        {
            throw new CompilerException(ErrorMessages.UnableToInferType);
        }
    }
}
