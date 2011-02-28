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

        public GFixTypeInfo TypeInfo { get; set; }

        public GFixCompiler(Named<IFunctor> functor, ModuleBuilder module, TypeBuilder utilityClass, MethodBuilder fmap, string moduleName)
        {
            this.functor = functor;
            this.module = module;
            this.utilityClass = utilityClass;
            this.fmap = fmap;
            this.moduleName = moduleName;
        }

        public void Compile()
        {
            TypeInfo = new GFixTypeInfo();

            TypeInfo.GreatestFixedPointFunction = module.DefineType(moduleName + "." + "Co" + functor.Name + "Function",
                TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);

            var resultType = TypeInfo.GreatestFixedPointFunction.DefineGenericParameters("R")[0];

            TypeInfo.GreatestFixedPointFunctionApplyMethod = TypeInfo.GreatestFixedPointFunction.DefineMethod("Apply",
                MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual);

            var underlyingType = TypeInfo.GreatestFixedPointFunctionApplyMethod.DefineGenericParameters("T")[0];

            var functorClass = new FunctorTypeMapper(underlyingType).Map(functor.Value);

            TypeInfo.GreatestFixedPointFunctionApplyMethod.SetParameters(underlyingType, typeof(IFunction<,>).MakeGenericType(underlyingType, functorClass));
            TypeInfo.GreatestFixedPointFunctionApplyMethod.SetReturnType(resultType);

            TypeInfo.GreatestFixedPoint = module.DefineType(moduleName + "." + "Co" + functor.Name,
                TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);

            TypeInfo.GreatestFixedPointApplyMethod = TypeInfo.GreatestFixedPoint.DefineMethod("Apply",
                MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual);

            var resultTypeGfix = TypeInfo.GreatestFixedPointApplyMethod.DefineGenericParameters("R")[0];

            TypeInfo.GreatestFixedPointApplyMethod.SetParameters(TypeInfo.GreatestFixedPointFunction.MakeGenericType(resultTypeGfix));
            TypeInfo.GreatestFixedPointApplyMethod.SetReturnType(resultTypeGfix);

            CompileAnaClass();

            CompileAna();

            CompileAnaFunction();

            CompileIn();

            CompileInFunction();

            CompileOut();

            CompileOutFunction();
        }

        private void CompileAna()
        {
            TypeInfo.Ana = utilityClass.DefineMethod("Ana", MethodAttributes.Public | MethodAttributes.Static);

            var seedType = TypeInfo.Ana.DefineGenericParameters("T")[0];

            TypeInfo.Ana.SetReturnType(TypeInfo.GreatestFixedPoint);

            var functorClass = new FunctorTypeMapper(seedType).Map(functor.Value);

            var generatorType = typeof(IFunction<,>).MakeGenericType(seedType, functorClass);

            TypeInfo.Ana.SetParameters(seedType, generatorType);

            var anaBody = TypeInfo.Ana.GetILGenerator();
            anaBody.Emit(OpCodes.Ldarg_0);
            anaBody.Emit(OpCodes.Ldarg_1);
            anaBody.Emit(OpCodes.Newobj, TypeBuilder.GetConstructor(TypeInfo.AnaClass.MakeGenericType(seedType), TypeInfo.AnaClassConstructor));
            anaBody.Emit(OpCodes.Ret);
        }

        private void CompileAnaClass()
        {
            TypeInfo.AnaClass = utilityClass.DefineNestedType("AnaClass",
                           TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
                           null, new[] { TypeInfo.GreatestFixedPoint });

            var anaClassGenericParameter = TypeInfo.AnaClass.DefineGenericParameters("T")[0];

            var anaClassGeneratorType = typeof(IFunction<,>).MakeGenericType(anaClassGenericParameter, new FunctorTypeMapper(anaClassGenericParameter).Map(functor.Value));

            var seedField = TypeInfo.AnaClass.DefineField("seed", anaClassGenericParameter, FieldAttributes.Private);
            var generatorField = TypeInfo.AnaClass.DefineField("generator", anaClassGeneratorType, FieldAttributes.Private);

            TypeInfo.AnaClassConstructor = TypeInfo.AnaClass.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { anaClassGenericParameter, anaClassGeneratorType });
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

            var apply = TypeInfo.AnaClass.DefineMethod("Apply", MethodAttributes.Public | MethodAttributes.Virtual);

            var resultType = apply.DefineGenericParameters("R")[0];

            apply.SetParameters(TypeInfo.GreatestFixedPointFunction.MakeGenericType(resultType));
            apply.SetReturnType(resultType);

            var applyMethodGenericClass = TypeBuilder.GetMethod(
                TypeInfo.GreatestFixedPointFunction.MakeGenericType(resultType), 
                TypeInfo.GreatestFixedPointFunctionApplyMethod);
            var applyMethodGenericMethod = applyMethodGenericClass.MakeGenericMethod(anaClassGenericParameter);

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

            TypeInfo.In = utilityClass.DefineMethod("In", MethodAttributes.Public | MethodAttributes.Static);

            TypeInfo.In.SetReturnType(new FunctorTypeMapper(TypeInfo.GreatestFixedPoint).Map(functor.Value));

            TypeInfo.In.SetParameters(TypeInfo.GreatestFixedPoint);

            TypeInfo.In.SetCustomAttribute(new CustomAttributeBuilder(typeof(ExtensionAttribute).GetConstructors()[0], new object[0]));

            var fGreatestFixedPoint = new FunctorTypeMapper(TypeInfo.GreatestFixedPoint).Map(functor.Value);

            var inBody = TypeInfo.In.GetILGenerator();
            inBody.Emit(OpCodes.Ldarg_0);
            inBody.Emit(OpCodes.Newobj, TypeInfo.InGeneratingFunctionConstructor);
            inBody.Emit(OpCodes.Callvirt, TypeInfo.GreatestFixedPointApplyMethod.MakeGenericMethod(fGreatestFixedPoint));
            inBody.Emit(OpCodes.Ret);
        }

        private void CompileInGeneratingFunction()
        {
            var fGreatestFixedPoint = new FunctorTypeMapper(TypeInfo.GreatestFixedPoint).Map(functor.Value);

            var inClass = utilityClass.DefineNestedType("InGeneratingFunction",
               TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
               null, new[] { TypeInfo.GreatestFixedPointFunction.MakeGenericType(fGreatestFixedPoint) });

            TypeInfo.InGeneratingFunctionConstructor = inClass.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var inCtorBody = TypeInfo.InGeneratingFunctionConstructor.GetILGenerator();
            inCtorBody.Emit(OpCodes.Ldarg_0);
            inCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            inCtorBody.Emit(OpCodes.Ret);

            var apply = inClass.DefineMethod("Apply", MethodAttributes.Public | MethodAttributes.Virtual,
                fGreatestFixedPoint, Type.EmptyTypes);

            var genericParameter = apply.DefineGenericParameters("T")[0];

            var fGenericParameter = new FunctorTypeMapper(genericParameter).Map(functor.Value);

            apply.SetParameters(genericParameter, typeof(IFunction<,>).MakeGenericType(genericParameter, fGenericParameter));

            var applyBody = apply.GetILGenerator();
            applyBody.Emit(OpCodes.Ldarg_2);
            applyBody.Emit(OpCodes.Newobj, TypeBuilder.GetConstructor(TypeInfo.AnaFunction.MakeGenericType(genericParameter),
                TypeInfo.AnaFunctionConstructor));
            applyBody.Emit(OpCodes.Call, fmap.MakeGenericMethod(genericParameter, TypeInfo.GreatestFixedPoint));
            applyBody.Emit(OpCodes.Ldarg_2);
            applyBody.Emit(OpCodes.Ldarg_1);
            applyBody.Emit(OpCodes.Callvirt, TypeBuilder.GetMethod(
                typeof(IFunction<,>).MakeGenericType(genericParameter, fGenericParameter),
                typeof(IFunction<,>).GetMethod("Call")));
            applyBody.Emit(OpCodes.Callvirt, TypeBuilder.GetMethod(
                typeof(IFunction<,>).MakeGenericType(fGenericParameter, fGreatestFixedPoint),
                typeof(IFunction<,>).GetMethod("Call")));
            applyBody.Emit(OpCodes.Ret);
        }

        private void CompileAnaFunction()
        {
            TypeInfo.AnaFunction = utilityClass.DefineNestedType("Ana",
                          TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
                          null, Type.EmptyTypes);

            var anaClassGenericParameter = TypeInfo.AnaFunction.DefineGenericParameters("T")[0];

            TypeInfo.AnaFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(anaClassGenericParameter,
                TypeInfo.GreatestFixedPoint));

            var seedType = typeof(IFunction<,>).MakeGenericType(anaClassGenericParameter, new FunctorTypeMapper(anaClassGenericParameter).Map(functor.Value));

            var seedField = TypeInfo.AnaFunction.DefineField("seed", seedType, FieldAttributes.Private);

            TypeInfo.AnaFunctionConstructor = TypeInfo.AnaFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { seedType });

            var anaCtorBody = TypeInfo.AnaFunctionConstructor.GetILGenerator();
            anaCtorBody.Emit(OpCodes.Ldarg_0);
            anaCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            anaCtorBody.Emit(OpCodes.Ldarg_0);
            anaCtorBody.Emit(OpCodes.Ldarg_1);
            anaCtorBody.Emit(OpCodes.Stfld, seedField);
            anaCtorBody.Emit(OpCodes.Ret);

            var call = TypeInfo.AnaFunction.DefineMethod("Call", MethodAttributes.Public | MethodAttributes.Virtual,
                TypeInfo.GreatestFixedPoint, new Type[] { anaClassGenericParameter });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Ldarg_0);
            callBody.Emit(OpCodes.Ldfld, seedField);
            callBody.Emit(OpCodes.Call, TypeInfo.Ana.MakeGenericMethod(anaClassGenericParameter));
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileOut()
        {
            TypeInfo.Out = utilityClass.DefineMethod("Out", MethodAttributes.Public | MethodAttributes.Static);

            var fGreatestFixedPoint = new FunctorTypeMapper(TypeInfo.GreatestFixedPoint).Map(functor.Value);

            TypeInfo.Out.SetParameters(fGreatestFixedPoint);

            TypeInfo.Out.SetReturnType(TypeInfo.GreatestFixedPoint);

            TypeInfo.Out.SetCustomAttribute(new CustomAttributeBuilder(typeof(ExtensionAttribute).GetConstructors()[0], new object[0]));

            var outBody = TypeInfo.Out.GetILGenerator();
            outBody.Emit(OpCodes.Ldarg_0);
            outBody.Emit(OpCodes.Newobj, TypeInfo.InFunctionConstructor);
            outBody.Emit(OpCodes.Call, fmap.MakeGenericMethod(TypeInfo.GreatestFixedPoint, fGreatestFixedPoint));
            outBody.Emit(OpCodes.Call, TypeInfo.Ana.MakeGenericMethod(fGreatestFixedPoint));
            outBody.Emit(OpCodes.Ret);
        }

        private void CompileInFunction()
        {
            var fGreatestFixedPoint = new FunctorTypeMapper(TypeInfo.GreatestFixedPoint).Map(functor.Value);

            TypeInfo.InFunction = utilityClass.DefineNestedType("InFunction",
               TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
               null, new[] { typeof(IFunction<,>).MakeGenericType(TypeInfo.GreatestFixedPoint, fGreatestFixedPoint) });

            TypeInfo.InFunctionConstructor = TypeInfo.InFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var inCtorBody = TypeInfo.InFunctionConstructor.GetILGenerator();
            inCtorBody.Emit(OpCodes.Ldarg_0);
            inCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            inCtorBody.Emit(OpCodes.Ret);

            var call = TypeInfo.InFunction.DefineMethod("Call", MethodAttributes.Public | MethodAttributes.Virtual,
                fGreatestFixedPoint, new Type[] { TypeInfo.GreatestFixedPoint });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Call, TypeInfo.In);
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileOutFunction()
        {
            var fGreatestFixedPoint = new FunctorTypeMapper(TypeInfo.GreatestFixedPoint).Map(functor.Value);

            TypeInfo.OutFunction = utilityClass.DefineNestedType("OutFunction",
               TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
               null, new[] { typeof(IFunction<,>).MakeGenericType(fGreatestFixedPoint, TypeInfo.GreatestFixedPoint) });

            TypeInfo.OutFunctionConstructor = TypeInfo.OutFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var outCtorBody = TypeInfo.OutFunctionConstructor.GetILGenerator();
            outCtorBody.Emit(OpCodes.Ldarg_0);
            outCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            outCtorBody.Emit(OpCodes.Ret);

            var call = TypeInfo.OutFunction.DefineMethod("Call", MethodAttributes.Public | MethodAttributes.Virtual,
                TypeInfo.GreatestFixedPoint, new Type[] { fGreatestFixedPoint });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Call, TypeInfo.Out);
            callBody.Emit(OpCodes.Ret);
        }
    }
}
