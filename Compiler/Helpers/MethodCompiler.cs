using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Modules;
using System.Reflection;
using Purity.Compiler.Data;
using Purity.Compiler.Types;
using Purity.Core;

namespace Purity.Compiler.Helpers
{
    public static class MethodCompiler
    {
        public static void Compile(string name, TypeBuilder dataClass, ITypedExpression typedExpression,
            DataDeclaration data, IMetadataContainer container, IRuntimeContainer runtimeContainer)
        {
            var method = dataClass.DefineMethod(name,
                MethodAttributes.Public | MethodAttributes.Static, null, Type.EmptyTypes);

            if (data.TypeParameters.Any())
            {
                method.DefineGenericParameters(data.TypeParameters.ToArray());
            }

            var converter = new TypeConverter(runtimeContainer, method.GetGenericArguments());
            var converted = converter.Convert(data.Type);
            method.SetReturnType(converted);

            var body = method.GetILGenerator();
            typedExpression.AcceptVisitor(new DataCompiler(body, method.GetGenericArguments(), runtimeContainer));
            body.Emit(OpCodes.Ret);

            container.Add(name, data);
            runtimeContainer.Add(name, method);

            RemoveFirstParameter(name, dataClass, method, new IType[0], data.Type, data.TypeParameters, runtimeContainer);
        }

        public static void Compile(string name, TypeBuilder dataClass, ConstructorInfo ctor, IType type, 
            string[] typeParameters, IMetadataContainer container, IRuntimeContainer runtimeContainer)
        {
            var method = dataClass.DefineMethod(name,
                 MethodAttributes.Public | MethodAttributes.Static, null, Type.EmptyTypes);

            if (typeParameters.Any())
            {
                method.DefineGenericParameters(typeParameters.ToArray());
            }

            var converter = new TypeConverter(runtimeContainer, method.GetGenericArguments());
            var converted = converter.Convert(type);
            method.SetReturnType(converted);

            var body = method.GetILGenerator();

            if (typeParameters.Any())
            {
                body.Emit(OpCodes.Newobj, TypeBuilder.GetConstructor(ctor.DeclaringType.MakeGenericType(method.GetGenericArguments()), ctor));
            }
            else
            {
                body.Emit(OpCodes.Newobj, ctor);
            }

            body.Emit(OpCodes.Ret);

            runtimeContainer.Add(name, method);

            var dataDecl = new DataDeclaration(type, null);
            dataDecl.TypeParameters = typeParameters;
            container.Add(name, dataDecl);

            RemoveFirstParameter(name, dataClass, method, new IType[0], type, typeParameters, runtimeContainer);
        }

        private static void RemoveFirstParameter(string name, TypeBuilder dataClass,
            MethodInfo method, IType[] arguments, IType returnType, string[] typeParameters,
            IRuntimeContainer runtimeContainer)
        {
            var arrow = returnType as ArrowType;

            if (arrow != null)
            {
                var newArguments = arguments.Concat(new[] { arrow.Left }).ToArray();
                var newReturnType = arrow.Right;

                var curried = dataClass.DefineMethod(name,
                    MethodAttributes.Public | MethodAttributes.Static,
                    null, Type.EmptyTypes);

                if (typeParameters.Any())
                {
                    curried.DefineGenericParameters(typeParameters.ToArray());
                }

                var converter = new TypeConverter(runtimeContainer, method.GetGenericArguments());
                var lastArgument = converter.Convert(arrow.Left);
                var compiledReturnType = converter.Convert(newReturnType);

                curried.SetReturnType(compiledReturnType);
                curried.SetParameters(newArguments.Select(converter.Convert).ToArray());

                var curriedBody = curried.GetILGenerator();

                for (int argIndex = 0; argIndex < newArguments.Length - 1; argIndex++)
                {
                    curriedBody.Emit(OpCodes.Ldarg, argIndex);
                }

                if (typeParameters.Any())
                {
                    curriedBody.Emit(OpCodes.Call, method.MakeGenericMethod(curried.GetGenericArguments()));
                }
                else
                {
                    curriedBody.Emit(OpCodes.Call, method);
                }

                curriedBody.Emit(OpCodes.Ldarg, newArguments.Length - 1);
                curriedBody.Emit(OpCodes.Callvirt, TypeBuilder.GetMethod(
                    typeof(IFunction<,>).MakeGenericType(lastArgument, compiledReturnType),
                    typeof(IFunction<,>).GetMethod(Constants.CallMethodName)));
                curriedBody.Emit(OpCodes.Ret);

                RemoveFirstParameter(name, dataClass, curried, newArguments, newReturnType, typeParameters, runtimeContainer);
            }
        }
    }
}
