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
using Purity.Core.Attributes;

namespace Purity.Compiler.Helpers
{
    public class LFixCompiler
    {
        private readonly Named<IFunctor> functor;
        private readonly ModuleBuilder module;
        private readonly TypeBuilder utilityClass;
        private readonly MethodBuilder fmap;
        private readonly string moduleName;
        private readonly string[] typeParameters;

        public LFixTypeInfo TypeInfo { get; set; }

        public LFixCompiler(Named<IFunctor> functor, ModuleBuilder module, TypeBuilder utilityClass, MethodBuilder fmap, string moduleName, string[] typeParameters)
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
            TypeInfo = new LFixTypeInfo();

            TypeInfo.Type = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + functor.Name,
                TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);

            ((TypeBuilder)TypeInfo.Type).SetCustomAttribute(new CustomAttributeBuilder(typeof(ExportAttribute).GetConstructors()[0], new object[0]));

            var genericParameters = typeParameters.Any() ? ((TypeBuilder)TypeInfo.Type).DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            TypeInfo.Cata = ((TypeBuilder)TypeInfo.Type).DefineMethod(Constants.CataMethodName,
                MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual);

            var cataReturnType = TypeInfo.Cata.DefineGenericParameters(Constants.CataMethodGenericParameterName)[0];

            var functorClass = new FunctorTypeMapper(cataReturnType, genericParameters).Map(functor.Value);

            TypeInfo.Cata.SetParameters(typeof(IFunction<,>).MakeGenericType(functorClass, cataReturnType));
            TypeInfo.Cata.SetReturnType(cataReturnType);

            CompileCataFunction();

            CompileCataFunction1();

            CompileOutClass();

            CompileOut();

            CompileOutFunction();

            CompileIn();

            CompileInFunction();
        }

        private void CompileOut()
        {
            TypeInfo.Out = utilityClass.DefineMethod(Constants.OutMethodName, MethodAttributes.Public | MethodAttributes.Static);

            var genericParameters = typeParameters.Any() ? TypeInfo.Out.DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            var returnType = typeParameters.Any() ? TypeInfo.Type.MakeGenericType(genericParameters.ToArray()) : TypeInfo.Type;

            var fLeastFixedPoint = new FunctorTypeMapper(returnType, genericParameters).Map(functor.Value);

            TypeInfo.Out.SetParameters(fLeastFixedPoint);

            TypeInfo.Out.SetReturnType(returnType);

            TypeInfo.Out.SetCustomAttribute(new CustomAttributeBuilder(typeof(ExtensionAttribute).GetConstructors()[0], new object[0]));

            var outBody = TypeInfo.Out.GetILGenerator();
            outBody.Emit(OpCodes.Ldarg_0);
            outBody.Emit(OpCodes.Newobj, genericParameters.Any()
                ? TypeBuilder.GetConstructor(
                    TypeInfo.OutClass.MakeGenericType(genericParameters),
                    TypeInfo.OutClassConstructor)
                : TypeInfo.OutClassConstructor);
            outBody.Emit(OpCodes.Ret);
        }

        private void CompileOutClass()
        {
            TypeInfo.OutClass = utilityClass.DefineNestedType(Constants.OutClassName,
               TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
               null, new[] { TypeInfo.Type });

            var genericParameters = typeParameters.Any() ? TypeInfo.OutClass.DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            var fLeastFixedPoint = new FunctorTypeMapper(TypeInfo.Type, genericParameters).Map(functor.Value);

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

            var fGenericParameter = new FunctorTypeMapper(genericParameter, genericParameters).Map(functor.Value);

            cata.SetParameters(typeof(IFunction<,>).MakeGenericType(fGenericParameter, genericParameter));
            cata.SetReturnType(genericParameter);

            var cataBody = cata.GetILGenerator();
            cataBody.Emit(OpCodes.Ldarg_1);
            cataBody.Emit(OpCodes.Ldarg_1);
            cataBody.Emit(OpCodes.Newobj, TypeBuilder.GetConstructor(TypeInfo.CataFunction.MakeGenericType(new[] { genericParameter }.Concat(genericParameters).ToArray()),
                TypeInfo.CataFunctionConstructor));
            cataBody.Emit(OpCodes.Call, fmap.MakeGenericMethod(new Type[] { TypeInfo.Type, genericParameter }.Concat(genericParameters).ToArray()));
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

        private void CompileCataFunction1()
        {
            TypeInfo.CataFunction1 = utilityClass.DefineNestedType(Constants.CataFunction1ClassName,
                           TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
                           null, Type.EmptyTypes);

            var genericParameters = TypeInfo.CataFunction1.DefineGenericParameters(new[] { Constants.CataFunction1ClassGenericParameterName }.Concat(typeParameters).ToArray());

            var fCataClassGenericParameter = new FunctorTypeMapper(genericParameters[0], genericParameters.Skip(1).ToArray()).Map(functor.Value);

            var inputParameter = typeParameters.Any() ? TypeInfo.Type.MakeGenericType(genericParameters.Skip(1).ToArray()) : TypeInfo.Type;

            var algebraType = typeof(IFunction<,>).MakeGenericType(fCataClassGenericParameter, genericParameters[0]);
            var initialMorphismType = typeof(IFunction<,>).MakeGenericType(inputParameter, genericParameters[0]);

            TypeInfo.CataFunction1.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(algebraType, initialMorphismType));

            TypeInfo.CataFunction1Constructor = TypeInfo.CataFunction1.DefineConstructor(MethodAttributes.Public,
                CallingConventions.Standard, Type.EmptyTypes);

            var cataCtorBody = TypeInfo.CataFunction1Constructor.GetILGenerator();
            cataCtorBody.Emit(OpCodes.Ldarg_0);
            cataCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            cataCtorBody.Emit(OpCodes.Ret);

            var call = TypeInfo.CataFunction1.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                initialMorphismType, new Type[] { algebraType });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Newobj, genericParameters.Any()
                ? TypeBuilder.GetConstructor(
                    TypeInfo.CataFunction.MakeGenericType(genericParameters),
                    TypeInfo.CataFunctionConstructor)
                : TypeInfo.CataFunctionConstructor);
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileCataFunction()
        {
            TypeInfo.CataFunction = utilityClass.DefineNestedType(Constants.CataFunctionClassName,
                           TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
                           null, Type.EmptyTypes);

            var genericParameters = TypeInfo.CataFunction.DefineGenericParameters(new[] { Constants.CataFunctionClassGenericParameterName }.Concat(typeParameters).ToArray());

            var inputParameter = typeParameters.Any() ? TypeInfo.Type.MakeGenericType(genericParameters.Skip(1).ToArray()) : TypeInfo.Type;

            TypeInfo.CataFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(inputParameter, genericParameters[0]));

            var fSeedType = new FunctorTypeMapper(genericParameters[0], genericParameters).Map(functor.Value);

            var algebra = typeof(IFunction<,>).MakeGenericType(fSeedType, genericParameters[0]);

            var seedField = TypeInfo.CataFunction.DefineField(Constants.CataFunctionClassSeedFieldName, algebra, FieldAttributes.Private);

            TypeInfo.CataFunctionConstructor = TypeInfo.CataFunction.DefineConstructor(MethodAttributes.Public,
                CallingConventions.Standard, new[] { algebra });

            var cataCtorBody = TypeInfo.CataFunctionConstructor.GetILGenerator();
            cataCtorBody.Emit(OpCodes.Ldarg_0);
            cataCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            cataCtorBody.Emit(OpCodes.Ldarg_0);
            cataCtorBody.Emit(OpCodes.Ldarg_1);
            cataCtorBody.Emit(OpCodes.Stfld, seedField);
            cataCtorBody.Emit(OpCodes.Ret);

            var call = TypeInfo.CataFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                genericParameters[0], new Type[] { inputParameter });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Ldarg_0);
            callBody.Emit(OpCodes.Ldfld, seedField);
            callBody.Emit(OpCodes.Callvirt, genericParameters.Skip(1).Any() 
                ? TypeBuilder.GetMethod(
                    TypeInfo.Type.MakeGenericType(genericParameters.Skip(1).ToArray()),
                    TypeInfo.Cata).MakeGenericMethod(genericParameters[0])
                : TypeInfo.Cata.MakeGenericMethod(genericParameters[0]));
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileIn()
        {
            TypeInfo.In = utilityClass.DefineMethod(Constants.InMethodName, MethodAttributes.Public | MethodAttributes.Static);

            var genericParameters = typeParameters.Any() ? TypeInfo.In.DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            var inputParameter = typeParameters.Any() ? TypeInfo.Type.MakeGenericType(genericParameters.ToArray()) : TypeInfo.Type;

            TypeInfo.In.SetReturnType(new FunctorTypeMapper(inputParameter, genericParameters).Map(functor.Value));

            TypeInfo.In.SetParameters(inputParameter);

            TypeInfo.In.SetCustomAttribute(new CustomAttributeBuilder(typeof(ExtensionAttribute).GetConstructors()[0], new object[0]));

            var inBody = TypeInfo.In.GetILGenerator();

            var fLeastFixedPoint = new FunctorTypeMapper(inputParameter, genericParameters).Map(functor.Value);

            inBody.Emit(OpCodes.Ldarg_0);
            inBody.Emit(OpCodes.Newobj, typeParameters.Any()
                ? TypeBuilder.GetConstructor(
                    TypeInfo.OutFunction.MakeGenericType(genericParameters),
                    TypeInfo.OutFunctionConstructor)
                : TypeInfo.OutFunctionConstructor);
            inBody.Emit(OpCodes.Call, fmap.MakeGenericMethod(new Type[] { fLeastFixedPoint, inputParameter }.Concat(genericParameters).ToArray()));
            inBody.Emit(OpCodes.Callvirt, typeParameters.Any()
                ? TypeBuilder.GetMethod(inputParameter, TypeInfo.Cata).MakeGenericMethod(fLeastFixedPoint)
                : TypeInfo.Cata.MakeGenericMethod(fLeastFixedPoint));
            inBody.Emit(OpCodes.Ret);
        }

        private void CompileOutFunction()
        {
            TypeInfo.OutFunction = utilityClass.DefineNestedType(Constants.OutFunctionClassName,
               TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic);

            var genericParameters = typeParameters.Any() ? TypeInfo.OutFunction.DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            var returnType = typeParameters.Any() ? TypeInfo.Type.MakeGenericType(genericParameters.ToArray()) : TypeInfo.Type;

            var fLeastFixedPoint = new FunctorTypeMapper(returnType, genericParameters).Map(functor.Value);

            TypeInfo.OutFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(fLeastFixedPoint, returnType));

            TypeInfo.OutFunctionConstructor = TypeInfo.OutFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var outCtorBody = TypeInfo.OutFunctionConstructor.GetILGenerator();
            outCtorBody.Emit(OpCodes.Ldarg_0);
            outCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            outCtorBody.Emit(OpCodes.Ret);

            var call = TypeInfo.OutFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                returnType, new Type[] { fLeastFixedPoint });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Call, typeParameters.Any() ? TypeInfo.Out.MakeGenericMethod(genericParameters) : TypeInfo.Out);
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileInFunction()
        {
            TypeInfo.InFunction = utilityClass.DefineNestedType(Constants.InFunctionClassName,
               TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic);

            var genericParameters = typeParameters.Any() ? TypeInfo.InFunction.DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            var inputParameter = typeParameters.Any() ? TypeInfo.Type.MakeGenericType(genericParameters.ToArray()) : TypeInfo.Type;

            var fLeastFixedPoint = new FunctorTypeMapper(inputParameter, genericParameters).Map(functor.Value);

            TypeInfo.InFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(inputParameter, fLeastFixedPoint));

            TypeInfo.InFunctionConstructor = TypeInfo.InFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var inCtorBody = TypeInfo.InFunctionConstructor.GetILGenerator();
            inCtorBody.Emit(OpCodes.Ldarg_0);
            inCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            inCtorBody.Emit(OpCodes.Ret);

            var call = TypeInfo.InFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                fLeastFixedPoint, new Type[] { inputParameter });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Call, typeParameters.Any() ? TypeInfo.In.MakeGenericMethod(genericParameters) : TypeInfo.In);
            callBody.Emit(OpCodes.Ret);
        }
    }
}
