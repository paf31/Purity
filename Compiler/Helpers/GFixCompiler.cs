using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Extensions;
using System.Reflection.Emit;
using System.Reflection;
using Purity.Core;
using Purity.Compiler.Modules;
using Purity.Compiler.Data;
using System.Runtime.CompilerServices;

namespace Purity.Compiler.Helpers
{
    public class GFixCompiler
    {
        private readonly Named<IFunctor> functor;
        private readonly ModuleBuilder module;
        private readonly TypeBuilder utilityClass;
        private readonly string moduleName;
        private readonly MethodBuilder fmap;
        private readonly string[] typeParameters;

        public GFixTypeInfo TypeInfo
        {
            get;
            set;
        }

        public GFixCompiler(Named<IFunctor> functor, ModuleBuilder module, TypeBuilder utilityClass, MethodBuilder fmap, string moduleName, string[] typeParameters)
        {
            this.functor = functor;
            this.module = module;
            this.utilityClass = utilityClass;
            this.fmap = fmap;
            this.moduleName = moduleName;
            this.typeParameters = typeParameters;
        }

        public void Compile()
        {
            TypeInfo = new GFixTypeInfo();

            TypeInfo.GreatestFixedPointFunction = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + functor.Name + Constants.FunctionSuffix,
                TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);

            var functionClassGenericParameters = TypeInfo.GreatestFixedPointFunction.DefineGenericParameters(new[] { Constants.GFixFunctionClassGenericParameterName }.Concat(typeParameters).ToArray());

            TypeInfo.GreatestFixedPointFunctionApplyMethod = TypeInfo.GreatestFixedPointFunction.DefineMethod(Constants.ApplyMethodName,
                MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual);

            var underlyingType = TypeInfo.GreatestFixedPointFunctionApplyMethod.DefineGenericParameters(Constants.ApplyMethodGenericParameterName)[0];

            var functorClass = new FunctorTypeMapper(underlyingType, functionClassGenericParameters.Skip(1).ToArray()).Map(functor.Value);

            TypeInfo.GreatestFixedPointFunctionApplyMethod.SetParameters(underlyingType, typeof(IFunction<,>).MakeGenericType(underlyingType, functorClass));
            TypeInfo.GreatestFixedPointFunctionApplyMethod.SetReturnType(functionClassGenericParameters[0]);

            TypeInfo.Type = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + functor.Name,
                TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);

            var typeGenericParameters = typeParameters.Any() ? TypeInfo.Type.DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            TypeInfo.GreatestFixedPointApplyMethod = TypeInfo.Type.DefineMethod(Constants.ApplyMethodName,
                MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual);

            var resultTypeGfix = TypeInfo.GreatestFixedPointApplyMethod.DefineGenericParameters(Constants.GFixFunctionClassGenericParameterName)[0];

            TypeInfo.GreatestFixedPointApplyMethod.SetParameters(TypeInfo.GreatestFixedPointFunction.MakeGenericType(new[] { resultTypeGfix }.Concat(typeGenericParameters).ToArray()));
            TypeInfo.GreatestFixedPointApplyMethod.SetReturnType(resultTypeGfix);

            CompileAnaClass();

            CompileAna();

            CompileAnaFunction();

            CompileAnaFunction1();

            CompileIn();

            CompileInFunction();

            CompileOut();

            CompileOutFunction();
        }

        private void CompileAna()
        {
            TypeInfo.Ana = utilityClass.DefineMethod(Constants.AnaMethodName, MethodAttributes.Public | MethodAttributes.Static);

            var anaGenericParameters = TypeInfo.Ana.DefineGenericParameters(new[] { Constants.AnaMethodGenericParameterName }.Concat(typeParameters).ToArray());

            TypeInfo.Ana.SetReturnType(typeParameters.Any() 
                ? TypeInfo.Type.MakeGenericType(anaGenericParameters.Skip(1).ToArray())
                : TypeInfo.Type);

            var functorClass = new FunctorTypeMapper(anaGenericParameters[0], anaGenericParameters.Skip(1).ToArray()).Map(functor.Value);

            var generatorType = typeof(IFunction<,>).MakeGenericType(anaGenericParameters[0], functorClass);

            TypeInfo.Ana.SetParameters(anaGenericParameters[0], generatorType);

            var anaBody = TypeInfo.Ana.GetILGenerator();
            anaBody.Emit(OpCodes.Ldarg_0);
            anaBody.Emit(OpCodes.Ldarg_1);
            anaBody.Emit(OpCodes.Newobj, TypeBuilder.GetConstructor(TypeInfo.AnaClass.MakeGenericType(anaGenericParameters), TypeInfo.AnaClassConstructor));
            anaBody.Emit(OpCodes.Ret);
        }

        private void CompileAnaClass()
        {
            TypeInfo.AnaClass = utilityClass.DefineNestedType(Constants.AnaClassName,
                           TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
                           null, Type.EmptyTypes);

            var anaClassGenericParameters = TypeInfo.AnaClass.DefineGenericParameters(new[] { Constants.AnaClassGenericParameterName }.Concat(typeParameters).ToArray());

            if (typeParameters.Any())
            {
                TypeInfo.AnaClass.AddInterfaceImplementation(TypeInfo.Type.MakeGenericType(anaClassGenericParameters.Skip(1).ToArray()));
            }
            else
            {
                TypeInfo.AnaClass.AddInterfaceImplementation(TypeInfo.Type);
            }

            var anaClassGeneratorType = typeof(IFunction<,>).MakeGenericType(anaClassGenericParameters[0],
                new FunctorTypeMapper(anaClassGenericParameters[0], anaClassGenericParameters.Skip(1).ToArray()).Map(functor.Value));

            var seedField = TypeInfo.AnaClass.DefineField(Constants.AnaClassSeedFieldName, anaClassGenericParameters[0], FieldAttributes.Private);
            var generatorField = TypeInfo.AnaClass.DefineField(Constants.AnaClassGeneratorFieldName, anaClassGeneratorType, FieldAttributes.Private);

            TypeInfo.AnaClassConstructor = TypeInfo.AnaClass.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { anaClassGenericParameters[0], anaClassGeneratorType });
            var ctorBody = TypeInfo.AnaClassConstructor.GetILGenerator();
            ctorBody.Emit(OpCodes.Ldarg_0);
            ctorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            ctorBody.Emit(OpCodes.Ldarg_0);
            ctorBody.Emit(OpCodes.Ldarg_1);
            ctorBody.Emit(OpCodes.Stfld, seedField);
            ctorBody.Emit(OpCodes.Ldarg_0);
            ctorBody.Emit(OpCodes.Ldarg_2);
            ctorBody.Emit(OpCodes.Stfld, generatorField);
            ctorBody.Emit(OpCodes.Ret);

            var apply = TypeInfo.AnaClass.DefineMethod(Constants.ApplyMethodName, MethodAttributes.Public | MethodAttributes.Virtual);

            var resultType = apply.DefineGenericParameters(Constants.GFixFunctionClassGenericParameterName)[0];

            var applyFunctionGenericParameters = new[] { resultType }.Concat(anaClassGenericParameters.Skip(1)).ToArray();

            apply.SetParameters(TypeInfo.GreatestFixedPointFunction.MakeGenericType(applyFunctionGenericParameters));
            apply.SetReturnType(resultType);

            var applyMethodGenericClass = TypeBuilder.GetMethod(
                TypeInfo.GreatestFixedPointFunction.MakeGenericType(applyFunctionGenericParameters),
                TypeInfo.GreatestFixedPointFunctionApplyMethod);
            var applyMethodGenericMethod = applyMethodGenericClass.MakeGenericMethod(anaClassGenericParameters[0]);

            var applyBody = apply.GetILGenerator();
            applyBody.Emit(OpCodes.Ldarg_1);
            applyBody.Emit(OpCodes.Ldarg_0);
            applyBody.Emit(OpCodes.Ldfld, seedField);
            applyBody.Emit(OpCodes.Ldarg_0);
            applyBody.Emit(OpCodes.Ldfld, generatorField);
            applyBody.Emit(OpCodes.Callvirt, applyMethodGenericMethod);
            applyBody.Emit(OpCodes.Ret);
        }

        private void CompileIn()
        {
            CompileInGeneratingFunction();

            TypeInfo.In = utilityClass.DefineMethod(Constants.InMethodName, MethodAttributes.Public | MethodAttributes.Static);

            var genericParameters = typeParameters.Any() ? TypeInfo.In.DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            var resultType = typeParameters.Any()
                ? TypeInfo.Type.MakeGenericType(genericParameters.ToArray())
                : TypeInfo.Type;

            TypeInfo.In.SetReturnType(new FunctorTypeMapper(resultType, genericParameters).Map(functor.Value));

            TypeInfo.In.SetParameters(resultType);

            TypeInfo.In.SetCustomAttribute(new CustomAttributeBuilder(typeof(ExtensionAttribute).GetConstructors()[0], new object[0]));

            var fGreatestFixedPoint = new FunctorTypeMapper(resultType, genericParameters).Map(functor.Value);

            var applyMethodGenericClass = typeParameters.Any()
                ? TypeBuilder.GetMethod(
                    TypeInfo.Type.MakeGenericType(genericParameters),
                    TypeInfo.GreatestFixedPointApplyMethod)
                : TypeInfo.GreatestFixedPointApplyMethod; 
            var applyMethodGenericMethod = applyMethodGenericClass.MakeGenericMethod(fGreatestFixedPoint);

            var inBody = TypeInfo.In.GetILGenerator();
            inBody.Emit(OpCodes.Ldarg_0);
            inBody.Emit(OpCodes.Newobj, typeParameters.Any()
                ? TypeBuilder.GetConstructor(
                    TypeInfo.InClass.MakeGenericType(genericParameters),
                    TypeInfo.InGeneratingFunctionConstructor)
                : TypeInfo.InGeneratingFunctionConstructor);
            inBody.Emit(OpCodes.Callvirt, applyMethodGenericMethod);
            inBody.Emit(OpCodes.Ret);
        }

        private void CompileInGeneratingFunction()
        {
            TypeInfo.InClass = utilityClass.DefineNestedType(Constants.InGeneratingFunctionClassName,
               TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic);

            var genericParameters = typeParameters.Any() ? TypeInfo.InClass.DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            var greatestFixedPoint = typeParameters.Any()
                ? TypeInfo.Type.MakeGenericType(genericParameters)
                : TypeInfo.Type;

            var fGreatestFixedPoint = new FunctorTypeMapper(greatestFixedPoint, genericParameters).Map(functor.Value);

            TypeInfo.InClass.AddInterfaceImplementation(TypeInfo.GreatestFixedPointFunction.MakeGenericType(new[] { fGreatestFixedPoint }.Concat(genericParameters).ToArray()));

            TypeInfo.InGeneratingFunctionConstructor = TypeInfo.InClass.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var inCtorBody = TypeInfo.InGeneratingFunctionConstructor.GetILGenerator();
            inCtorBody.Emit(OpCodes.Ldarg_0);
            inCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            inCtorBody.Emit(OpCodes.Ret);

            var apply = TypeInfo.InClass.DefineMethod(Constants.ApplyMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                fGreatestFixedPoint, Type.EmptyTypes);

            var genericParameter = apply.DefineGenericParameters(Constants.ApplyMethodGenericParameterName)[0];

            var fGenericParameter = new FunctorTypeMapper(genericParameter, genericParameters).Map(functor.Value);

            apply.SetParameters(genericParameter, typeof(IFunction<,>).MakeGenericType(genericParameter, fGenericParameter));

            var applyBody = apply.GetILGenerator();
            applyBody.Emit(OpCodes.Ldarg_2);
            applyBody.Emit(OpCodes.Newobj, TypeBuilder.GetConstructor(
                TypeInfo.AnaFunction.MakeGenericType(new[] { genericParameter }.Concat(genericParameters).ToArray()),
                TypeInfo.AnaFunctionConstructor));
            applyBody.Emit(OpCodes.Call, fmap.MakeGenericMethod(new Type[] { genericParameter, greatestFixedPoint }.Concat(genericParameters).ToArray()));
            applyBody.Emit(OpCodes.Ldarg_2);
            applyBody.Emit(OpCodes.Ldarg_1);
            applyBody.Emit(OpCodes.Callvirt, TypeBuilder.GetMethod(
                typeof(IFunction<,>).MakeGenericType(genericParameter, fGenericParameter),
                typeof(IFunction<,>).GetMethod(Constants.CallMethodName)));
            applyBody.Emit(OpCodes.Callvirt, TypeBuilder.GetMethod(
                typeof(IFunction<,>).MakeGenericType(fGenericParameter, fGreatestFixedPoint),
                typeof(IFunction<,>).GetMethod(Constants.CallMethodName)));
            applyBody.Emit(OpCodes.Ret);
        }

        private void CompileAnaFunction()
        {
            TypeInfo.AnaFunction = utilityClass.DefineNestedType(Constants.AnaFunctionClassName,
                          TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
                          null, Type.EmptyTypes);

            var anaClassGenericParameters = TypeInfo.AnaFunction.DefineGenericParameters(new[] { Constants.AnaFunctionClassGenericParameterName }.Concat(typeParameters).ToArray());

            var resultType = typeParameters.Any() 
                ? TypeInfo.Type.MakeGenericType(anaClassGenericParameters.Skip(1).ToArray())
                : TypeInfo.Type;

            TypeInfo.AnaFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(anaClassGenericParameters[0],
                resultType));

            var seedType = typeof(IFunction<,>).MakeGenericType(anaClassGenericParameters[0],
                new FunctorTypeMapper(anaClassGenericParameters[0], anaClassGenericParameters.Skip(1).ToArray()).Map(functor.Value));

            var seedField = TypeInfo.AnaFunction.DefineField(Constants.AnaFunctionClassSeedFieldName, seedType, FieldAttributes.Private);

            TypeInfo.AnaFunctionConstructor = TypeInfo.AnaFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { seedType });

            var anaCtorBody = TypeInfo.AnaFunctionConstructor.GetILGenerator();
            anaCtorBody.Emit(OpCodes.Ldarg_0);
            anaCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            anaCtorBody.Emit(OpCodes.Ldarg_0);
            anaCtorBody.Emit(OpCodes.Ldarg_1);
            anaCtorBody.Emit(OpCodes.Stfld, seedField);
            anaCtorBody.Emit(OpCodes.Ret);

            var call = TypeInfo.AnaFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                resultType, new[] { anaClassGenericParameters[0] });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Ldarg_0);
            callBody.Emit(OpCodes.Ldfld, seedField);
            callBody.Emit(OpCodes.Call, TypeInfo.Ana.MakeGenericMethod(anaClassGenericParameters));
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileAnaFunction1()
        {
            TypeInfo.AnaFunction1 = utilityClass.DefineNestedType(Constants.AnaFunction1ClassName,
                          TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
                          null, Type.EmptyTypes);

            var anaClassGenericParameters = TypeInfo.AnaFunction1.DefineGenericParameters(new[] { Constants.AnaFunction1ClassGenericParameterName }.Concat(typeParameters).ToArray());
            
            var resultType = typeParameters.Any()
                ? TypeInfo.Type.MakeGenericType(anaClassGenericParameters.Skip(1).ToArray())
                : TypeInfo.Type;

            var fAnaClassGenericParameter = new FunctorTypeMapper(anaClassGenericParameters[0], anaClassGenericParameters.Skip(1).ToArray()).Map(functor.Value);

            var coalgebraType = typeof(IFunction<,>).MakeGenericType(anaClassGenericParameters[0], fAnaClassGenericParameter);
            var terminalMorphismType = typeof(IFunction<,>).MakeGenericType(anaClassGenericParameters[0], resultType);

            TypeInfo.AnaFunction1.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(coalgebraType,
                terminalMorphismType));

            TypeInfo.AnaFunction1Constructor = TypeInfo.AnaFunction1.DefineConstructor(MethodAttributes.Public,
                CallingConventions.Standard, Type.EmptyTypes);

            var anaCtorBody = TypeInfo.AnaFunction1Constructor.GetILGenerator();
            anaCtorBody.Emit(OpCodes.Ldarg_0);
            anaCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            anaCtorBody.Emit(OpCodes.Ret);

            var call = TypeInfo.AnaFunction1.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                terminalMorphismType, new Type[] { coalgebraType });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Newobj, TypeBuilder.GetConstructor(
                TypeInfo.AnaFunction.MakeGenericType(anaClassGenericParameters),
                TypeInfo.AnaFunctionConstructor));
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileOut()
        {
            TypeInfo.Out = utilityClass.DefineMethod(Constants.OutMethodName, MethodAttributes.Public | MethodAttributes.Static);

            var genericParameters = typeParameters.Any() ? TypeInfo.Out.DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            var returnType = typeParameters.Any()
                ? TypeInfo.Type.MakeGenericType(genericParameters)
                : TypeInfo.Type;

            var fGreatestFixedPoint = new FunctorTypeMapper(returnType, genericParameters).Map(functor.Value);

            TypeInfo.Out.SetParameters(fGreatestFixedPoint);

            TypeInfo.Out.SetReturnType(returnType);

            TypeInfo.Out.SetCustomAttribute(new CustomAttributeBuilder(typeof(ExtensionAttribute).GetConstructors()[0], new object[0]));

            var outBody = TypeInfo.Out.GetILGenerator();
            outBody.Emit(OpCodes.Ldarg_0);
            outBody.Emit(OpCodes.Newobj, typeParameters.Any()
                ? TypeBuilder.GetConstructor(
                    TypeInfo.InFunction.MakeGenericType(genericParameters), 
                    TypeInfo.InFunctionConstructor)
                : TypeInfo.InFunctionConstructor);
            outBody.Emit(OpCodes.Call, fmap.MakeGenericMethod(new[] { returnType, fGreatestFixedPoint }.Concat(genericParameters).ToArray()));
            outBody.Emit(OpCodes.Call, TypeInfo.Ana.MakeGenericMethod(new[] { fGreatestFixedPoint }.Concat(genericParameters).ToArray()));
            outBody.Emit(OpCodes.Ret);
        }

        private void CompileInFunction()
        {
            TypeInfo.InFunction = utilityClass.DefineNestedType(Constants.InFunctionClassName,
               TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic);

            var genericParameters = typeParameters.Any() ? TypeInfo.InFunction.DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            var fGreatestFixedPoint = new FunctorTypeMapper(TypeInfo.Type, genericParameters).Map(functor.Value);

            TypeInfo.InFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(TypeInfo.Type, fGreatestFixedPoint));

            TypeInfo.InFunctionConstructor = TypeInfo.InFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var inCtorBody = TypeInfo.InFunctionConstructor.GetILGenerator();
            inCtorBody.Emit(OpCodes.Ldarg_0);
            inCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            inCtorBody.Emit(OpCodes.Ret);

            var call = TypeInfo.InFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                fGreatestFixedPoint, new Type[] { TypeInfo.Type });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Call, typeParameters.Any() 
                ? TypeInfo.In.MakeGenericMethod(genericParameters)
                : TypeInfo.In);
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileOutFunction()
        {
            TypeInfo.OutFunction = utilityClass.DefineNestedType(Constants.OutFunctionClassName,
               TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic);

            var genericParameters = typeParameters.Any() ? TypeInfo.OutFunction.DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            var fGreatestFixedPoint = new FunctorTypeMapper(TypeInfo.Type, genericParameters).Map(functor.Value);

            TypeInfo.OutFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(fGreatestFixedPoint, TypeInfo.Type));

            TypeInfo.OutFunctionConstructor = TypeInfo.OutFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var outCtorBody = TypeInfo.OutFunctionConstructor.GetILGenerator();
            outCtorBody.Emit(OpCodes.Ldarg_0);
            outCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            outCtorBody.Emit(OpCodes.Ret);

            var call = TypeInfo.OutFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                TypeInfo.Type, new Type[] { fGreatestFixedPoint });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Call, typeParameters.Any() 
                ? TypeInfo.Out.MakeGenericMethod(genericParameters)
                : TypeInfo.Out);
            callBody.Emit(OpCodes.Ret);
        }
    }
}
