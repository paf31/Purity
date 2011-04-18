using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class TypeConverter : IPartialTypeVisitor<IType>
    {
        public static IType Convert(IPartialType type)
        {
            return type.AcceptVisitor(new TypeConverter());
        }

        public IType VisitArrow(Types.ArrowType t)
        {
            return new Purity.Compiler.Types.ArrowType(Convert(t.Left), Convert(t.Right));
        }

        public IType VisitSynonym(Types.TypeSynonym t)
        {
            return new Purity.Compiler.Types.TypeSynonym(t.Identifier, t.TypeParameters.Select(Convert).ToArray());
        }

        public IType VisitProduct(Types.ProductType t)
        {
            return new Purity.Compiler.Types.ProductType(Convert(t.Left), Convert(t.Right));
        }

        public IType VisitSum(Types.SumType t)
        {
            return new Purity.Compiler.Types.SumType(Convert(t.Left), Convert(t.Right));
        }

        public IType VisitParameter(Types.TypeParameter t)
        {
            return new Purity.Compiler.Types.TypeParameter(t.Identifier);
        }

        public IType VisitUnknown(Types.UnknownType t)
        {
            throw new CompilerException(ErrorMessages.UnableToInferType);
        }
    }
}
