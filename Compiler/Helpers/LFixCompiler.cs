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
    public class LFixCompiler
    {
        private readonly Named<IFunctor> functor;
        private readonly ModuleBuilder module;
        private readonly TypeBuilder utilityClass;
        private readonly MethodBuilder fmap;
        private readonly string moduleName;

        public LFixTypeInfo TypeInfo { get; set; }

        public LFixCompiler(Named<IFunctor> functor, ModuleBuilder module, TypeBuilder utilityClass, MethodBuilder fmap, string moduleName)
        {
            this.functor = functor;
            this.module = module;
            this.utilityClass = utilityClass;
            this.fmap = fmap;
            this.moduleName = moduleName;
        }

        public void Compile()
        {
            TypeInfo = new LFixTypeInfo();

            TypeInfo.Type = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + functor.Name,
                TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);

            TypeInfo.Cata = TypeInfo.Type.DefineMethod(Constants.CataMethodName,
                MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual);

            var cataReturnType = TypeInfo.Cata.DefineGenericParameters(Constants.CataMethodGenericParameterName)[0];

            var functorClass = new FunctorTypeMapper(cataReturnType).Map(functor.Value);

            TypeInfo.Cata.SetParameters(typeof(IFunction<,>).MakeGenericType(functorClass, cataReturnType));
            TypeInfo.Cata.SetReturnType(cataReturnType);

            CompileCataFunction();

            CompileOutClass();

            CompileOut();

            CompileOutFunction();

            CompileIn();

            CompileInFunction();
        }

        private void CompileOut()
        {
            TypeInfo.Out = utilityClass.DefineMethod(Constants.OutMethodName, MethodAttributes.Public | MethodAttributes.Static);

            var fLeastFixedPoint = new FunctorTypeMapper(TypeInfo.Type).Map(functor.Value);

            TypeInfo.Out.SetParameters(fLeastFixedPoint);

            TypeInfo.Out.SetReturnType(TypeInfo.Type);

            TypeInfo.Out.SetCustomAttribute(new CustomAttributeBuilder(typeof(ExtensionAttribute).GetConstructors()[0], new object[0]));

            var outBody = TypeInfo.Out.GetILGenerator();
            outBody.Emit(OpCodes.Ldarg_0);
            outBody.Emit(OpCodes.Newobj, TypeInfo.OutClassConstructor);
            outBody.Emit(OpCodes.Ret);
        }

        private void CompileOutClass()
        {
            var fLeastFixedPoint = new FunctorTypeMapper(TypeInfo.Type).Map(functor.Value);

            TypeInfo.OutClass = utilityClass.DefineNestedType(Constants.OutClassName,
               TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
               null, new[] { TypeInfo.Type });

            var predField = TypeInfo.OutClass.DefineField(Constants.OutClassPredFieldName, fLeastFixedPoint, FieldAttributes.Private);

            TypeInfo.OutClassConstructor = TypeInfo.OutClass.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { fLeastFixedPoint });

            var outCtorBody = TypeInfo.OutClassConstructor.GetILGenerator();
            outCtorBody.Emit(OpCodes.Ldarg_0);
            outCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            outCtorBody.Emit(OpCodes.Ldarg_0);
            outCtorBody.Emit(OpCodes.Ldarg_1);
            outCtorBody.Emit(OpCodes.Stfld, predField);
            outCtorBody.Emit(OpCodes.Ret);

            var cata = TypeInfo.OutClass.DefineMethod(Constants.CataMethodName, MethodAttributes.Public | MethodAttributes.Virtual);

            var genericParameter = cata.DefineGenericParameters(Constants.CataMethodGenericParameterName)[0];

            var fGenericParameter = new FunctorTypeMapper(genericParameter).Map(functor.Value);

            cata.SetParameters(typeof(IFunction<,>).MakeGenericType(fGenericParameter, genericParameter));
            cata.SetReturnType(genericParameter);

            var cataBody = cata.GetILGenerator();
            cataBody.Emit(OpCodes.Ldarg_1);
            cataBody.Emit(OpCodes.Ldarg_1);
            cataBody.Emit(OpCodes.Newobj, TypeBuilder.GetConstructor(TypeInfo.CataFunction.MakeGenericType(genericParameter),
                TypeInfo.CataFunctionConstructor));
            cataBody.Emit(OpCodes.Call, fmap.MakeGenericMethod(TypeInfo.Type, genericParameter));
            cataBody.Emit(OpCodes.Ldarg_0);
            cataBody.Emit(OpCodes.Ldfld, predField);
            cataBody.Emit(OpCodes.Callvirt, TypeBuilder.GetMethod(
                typeof(IFunction<,>).MakeGenericType(fLeastFixedPoint, fGenericParameter),
                typeof(IFunction<,>).GetMethod(Constants.CallMethodName)));
            cataBody.Emit(OpCodes.Callvirt, TypeBuilder.GetMethod(
                typeof(IFunction<,>).MakeGenericType(fGenericParameter, genericParameter),
                typeof(IFunction<,>).GetMethod(Constants.CallMethodName)));
            cataBody.Emit(OpCodes.Ret);
        }

        private void CompileCataFunction()
        {
            TypeInfo.CataFunction = utilityClass.DefineNestedType(Constants.CataFunctionClassName,
                           TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
                           null, Type.EmptyTypes);

            var cataClassGenericParameter = TypeInfo.CataFunction.DefineGenericParameters(Constants.CataFunctionClassGenericParameterName)[0];

            TypeInfo.CataFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(TypeInfo.Type, cataClassGenericParameter));

            var seedType = typeof(IFunction<,>).MakeGenericType(new FunctorTypeMapper(cataClassGenericParameter).Map(functor.Value), cataClassGenericParameter);

            var seedField = TypeInfo.CataFunction.DefineField(Constants.CataFunctionClassSeedFieldName, seedType, FieldAttributes.Private);

            TypeInfo.CataFunctionConstructor = TypeInfo.CataFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { seedType });

            var cataCtorBody = TypeInfo.CataFunctionConstructor.GetILGenerator();
            cataCtorBody.Emit(OpCodes.Ldarg_0);
            cataCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            cataCtorBody.Emit(OpCodes.Ldarg_0);
            cataCtorBody.Emit(OpCodes.Ldarg_1);
            cataCtorBody.Emit(OpCodes.Stfld, seedField);
            cataCtorBody.Emit(OpCodes.Ret);

            var call = TypeInfo.CataFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                cataClassGenericParameter, new Type[] { TypeInfo.Type });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Ldarg_0);
            callBody.Emit(OpCodes.Ldfld, seedField);
            callBody.Emit(OpCodes.Callvirt, TypeInfo.Cata.MakeGenericMethod(cataClassGenericParameter));
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileIn()
        {
            TypeInfo.In = utilityClass.DefineMethod(Constants.InMethodName, MethodAttributes.Public | MethodAttributes.Static);

            TypeInfo.In.SetReturnType(new FunctorTypeMapper(TypeInfo.Type).Map(functor.Value));

            TypeInfo.In.SetParameters(TypeInfo.Type);

            TypeInfo.In.SetCustomAttribute(new CustomAttributeBuilder(typeof(ExtensionAttribute).GetConstructors()[0], new object[0]));
            
            var inBody = TypeInfo.In.GetILGenerator();

            var fLeastFixedPoint = new FunctorTypeMapper(TypeInfo.Type).Map(functor.Value);

            inBody.Emit(OpCodes.Ldarg_0);
            inBody.Emit(OpCodes.Newobj, TypeInfo.OutFunctionConstructor);
            inBody.Emit(OpCodes.Call, fmap.MakeGenericMethod(fLeastFixedPoint, TypeInfo.Type));
            inBody.Emit(OpCodes.Callvirt, TypeInfo.Cata.MakeGenericMethod(fLeastFixedPoint));
            inBody.Emit(OpCodes.Ret);
        }

        private void CompileOutFunction()
        {
            var fLeastFixedPoint = new FunctorTypeMapper(TypeInfo.Type).Map(functor.Value);

            TypeInfo.OutFunction = utilityClass.DefineNestedType(Constants.OutFunctionClassName,
               TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
               null, new[] { typeof(IFunction<,>).MakeGenericType(fLeastFixedPoint, TypeInfo.Type) });

            TypeInfo.OutFunctionConstructor = TypeInfo.OutFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var outCtorBody = TypeInfo.OutFunctionConstructor.GetILGenerator();
            outCtorBody.Emit(OpCodes.Ldarg_0);
            outCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            outCtorBody.Emit(OpCodes.Ret);

            var call = TypeInfo.OutFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                TypeInfo.Type, new Type[] { fLeastFixedPoint });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Call, TypeInfo.Out);
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileInFunction()
        {
            var fLeastFixedPoint = new FunctorTypeMapper(TypeInfo.Type).Map(functor.Value);

            TypeInfo.InFunction = utilityClass.DefineNestedType(Constants.InFunctionClassName,
               TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
               null, new[] { typeof(IFunction<,>).MakeGenericType(TypeInfo.Type, fLeastFixedPoint) });

            TypeInfo.InFunctionConstructor = TypeInfo.InFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var inCtorBody = TypeInfo.InFunctionConstructor.GetILGenerator();
            inCtorBody.Emit(OpCodes.Ldarg_0);
            inCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            inCtorBody.Emit(OpCodes.Ret);

            var call = TypeInfo.InFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                fLeastFixedPoint, new Type[] { TypeInfo.Type });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Call, TypeInfo.In);
            callBody.Emit(OpCodes.Ret);
        }
    }
}
