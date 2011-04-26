using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Extensions;
using Purity.Compiler.Interfaces;
using Purity.Core;
using System.Reflection.Emit;
using Purity.Compiler.Functors;
using Purity.Compiler.Data;
using Purity.Core.Types;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Helpers
{
    public class TypeConverter : ITypeVisitor<Type>
    {
        private readonly Type[] genericMethodParameters;
       
        public TypeConverter(Type[] genericMethodParameters)
        {
            this.genericMethodParameters = genericMethodParameters;
        }

        public Type Convert(IType t)
        {
            return t.AcceptVisitor(this);
        }

        public Type VisitArrow(Types.ArrowType t)
        {
            return typeof(IFunction<,>).MakeGenericType(Convert(t.Left), Convert(t.Right));
        }

        public Type VisitProduct(Types.ProductType t)
        {
            return typeof(Tuple<,>).MakeGenericType(Convert(t.Left), Convert(t.Right));
        }

        public Type VisitSum(Types.SumType t)
        {
            return typeof(Either<,>).MakeGenericType(Convert(t.Left), Convert(t.Right));
        }

        public Type VisitSynonym(Types.TypeSynonym t)
        {
            var type = TypeContainer.ResolveType(t.Identifier).Type;
            return t.TypeParameters.Any() ? type.MakeGenericType(t.TypeParameters.Select(Convert).ToArray()) : type;
        }

        public Type VisitParameter(Types.TypeParameter t)
        {
            var genericParameter = genericMethodParameters.FirstOrDefault(p => p.Name.Equals(t.Identifier));

            if (genericParameter == null) 
            {
                throw new CompilerException(ErrorMessages.UnknownTypeParameter);
            }

            return genericParameter;
        }
    }
}
