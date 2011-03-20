using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class PartialTypeCreator : ITypeVisitor<IPartialType>
    {
        public static IPartialType Convert(IType type)
        {
            return type.AcceptVisitor(new PartialTypeCreator());
        }

        public IPartialType VisitArrow(Compiler.Types.ArrowType t)
        {
            return new Types.ArrowType(Convert(t.Left), Convert(t.Right));
        }

        public IPartialType VisitSynonym(Compiler.Types.TypeSynonym t)
        {
            return new Types.TypeSynonym(t.Identifier);
        }

        public IPartialType VisitProduct(Compiler.Types.ProductType t)
        {
            return new Types.ProductType(Convert(t.Left), Convert(t.Right));
        }

        public IPartialType VisitSum(Compiler.Types.SumType t)
        {
            return new Types.SumType(Convert(t.Left), Convert(t.Right));
        }

        public IPartialType VisitLFix(Compiler.Types.LFixType t)
        {
            var result = new Types.LFixType(PartialFunctorCreator.Convert(t.Functor));
            result.Identifier = t.Identifier;
            return result;
        }

        public IPartialType VisitGFix(Compiler.Types.GFixType t)
        {
            var result = new Types.GFixType(PartialFunctorCreator.Convert(t.Functor));
            result.Identifier = t.Identifier;
            return result;
        }

        public IPartialType VisitParameter(Compiler.Types.TypeParameter t)
        {
            return new Types.TypeParameter(t.Identifier);
        }
    }
}
