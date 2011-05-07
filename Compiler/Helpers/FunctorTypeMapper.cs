using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Core;
using Purity.Core.Types;

namespace Purity.Compiler.Helpers
{
    public class FunctorTypeMapper : ITypeVisitor<Type>
    {
        private readonly Type type;
        private readonly Type[] genericParameters;
        private readonly string variableName;
        private readonly IRuntimeContainer runtimeContainer;

        public FunctorTypeMapper(string variableName, Type type, Type[] genericParameters, IRuntimeContainer runtimeContainer)
        {
            this.type = type;
            this.variableName = variableName;
            this.genericParameters = genericParameters;
            this.runtimeContainer = runtimeContainer;
        }

        public static Type Map(IType functorType, string variableName, Type type, Type[] genericParameters, IRuntimeContainer runtimeContainer)
        {
            var visitor = new FunctorTypeMapper(variableName, type, genericParameters, runtimeContainer);
            return functorType.AcceptVisitor(visitor);
        }

        public Type VisitArrow(Types.ArrowType t)
        {
            return typeof(IFunction<,>).MakeGenericType(t.Left.AcceptVisitor(this),
                t.Right.AcceptVisitor(this));
        }

        public Type VisitSynonym(Types.TypeSynonym t)
        {
            return new TypeConverter(runtimeContainer, genericParameters).Convert(t);
        }

        public Type VisitProduct(Types.ProductType t)
        {
            return typeof(Tuple<,>).MakeGenericType(t.Left.AcceptVisitor(this),
                t.Right.AcceptVisitor(this));
        }

        public Type VisitSum(Types.SumType t)
        {
            return typeof(Either<,>).MakeGenericType(t.Left.AcceptVisitor(this),
                t.Right.AcceptVisitor(this));
        }

        public Type VisitParameter(Types.TypeParameter t)
        {
            if (t.Identifier.Equals(variableName))
            {
                return type;
            }
            else 
            {
                return new TypeConverter(runtimeContainer, genericParameters).Convert(t);
            }
        }
    }
}
