using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Core;
using Purity.Core.Types;
using System.Reflection;

namespace Purity.Compiler.Utilities
{
    public static class ReflectionUtilities
    {
        public static IType InferType(Type type)
        {
            Type genericTypeDefinition = type.GetGenericArguments().Any() ? type.GetGenericTypeDefinition() : type;

            if (typeof(IFunction<,>).IsAssignableFrom(genericTypeDefinition))
            {
                Type left = type.GetGenericArguments()[0];
                Type right = type.GetGenericArguments()[1];
                return new Types.ArrowType(InferType(left), InferType(right));
            }
            else if (typeof(Either<,>).IsAssignableFrom(genericTypeDefinition))
            {
                Type left = type.GetGenericArguments()[0];
                Type right = type.GetGenericArguments()[1];
                return new Types.SumType(InferType(left), InferType(right));
            }
            else if (typeof(Tuple<,>).IsAssignableFrom(genericTypeDefinition))
            {
                Type left = type.GetGenericArguments()[0];
                Type right = type.GetGenericArguments()[1];
                return new Types.ProductType(InferType(left), InferType(right));
            }
            else if (type.IsGenericParameter)
            {
                return new Types.TypeParameter(type.Name);
            }
            else
            {
                Container.ResolveType(type.Name);
                return new Types.TypeSynonym(type.Name, type.GetGenericArguments().Select(InferType).ToArray());
            }
        }

        public static IType InferFunctorFromFiniteType(MethodInfo cataMethod, string variableName)
        {
            var algebraType = cataMethod.GetParameters()[0].ParameterType;
            var fCarrierType = algebraType.GetGenericArguments()[0];

            return InferFunctor(fCarrierType, variableName, Constants.CataMethodGenericParameterName);
        }

        public static IType InferFunctorFromInfiniteType(MethodInfo applyMethod, string variableName)
        {
            var functionType = applyMethod.GetParameters()[0];
            var functionTypeApplyMethod = functionType.ParameterType.GetMethod(Constants.ApplyMethodName);
            var generator = functionTypeApplyMethod.GetParameters()[1];
            var fCarrierType = generator.ParameterType.GetGenericArguments()[1];

            return InferFunctor(fCarrierType, variableName, Constants.ApplyMethodGenericParameterName);
        }

        public static IType InferFunctor(Type type, string variableName, string genericParameterName)
        {
            Type genericTypeDefinition = type.GetGenericArguments().Any() ? type.GetGenericTypeDefinition() : type;

            if (genericTypeDefinition.IsGenericParameter)
            {
                if (genericTypeDefinition.Name.Equals(genericParameterName))
                {
                    return new Types.TypeParameter(variableName);
                }
                else
                {
                    return new Types.TypeParameter(type.Name);
                }
            }
            else if (typeof(IFunction<,>).IsAssignableFrom(genericTypeDefinition))
            {
                Type left = type.GetGenericArguments()[0];
                Type right = type.GetGenericArguments()[1];
                return new Types.ArrowType(InferType(left), InferFunctor(right, variableName, genericParameterName));
            }
            else if (typeof(Either<,>).IsAssignableFrom(genericTypeDefinition))
            {
                Type left = type.GetGenericArguments()[0];
                Type right = type.GetGenericArguments()[1];
                return new Types.SumType(InferFunctor(left, variableName, genericParameterName), 
                    InferFunctor(right, variableName, genericParameterName));
            }
            else if (typeof(Tuple<,>).IsAssignableFrom(genericTypeDefinition))
            {
                Type left = type.GetGenericArguments()[0];
                Type right = type.GetGenericArguments()[1];
                return new Types.ProductType(InferFunctor(left, variableName, genericParameterName), 
                    InferFunctor(right, variableName, genericParameterName));
            }
            else
            {
                Container.ResolveType(type.Name);
                return new Types.TypeSynonym(type.Name, type.GetGenericArguments().Select(InferType).ToArray());
            }
        }
    }
}
