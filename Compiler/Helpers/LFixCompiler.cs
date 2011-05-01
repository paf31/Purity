using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using System.Reflection.Emit;
using System.Reflection;
using Purity.Core;
using Purity.Compiler.Modules;
using Purity.Compiler.Data;
using System.Runtime.CompilerServices;
using Purity.Core.Attributes;
using Purity.Compiler.TypeDeclarations;

namespace Purity.Compiler.Helpers
{
    public class LFixCompiler
    {
        private readonly string name;
        private readonly LFixTypeDeclaration declaration;
        private readonly ModuleBuilder module;
        private readonly TypeBuilder utilityClass;
        private readonly MethodBuilder fmap;
        private readonly string moduleName;
        private MethodBuilder Cata;
        private MethodBuilder In;
        private MethodBuilder Out;
        private TypeBuilder CataFunction;
        private ConstructorBuilder CataFunctionConstructor;
        private TypeBuilder CataFunction1;
        private TypeBuilder OutFunction;
        private TypeBuilder OutClass;
        private ConstructorBuilder OutClassConstructor;

        public ConstructorBuilder CataFunction1Constructor
        {
            get;
            set;
        }

        public ConstructorBuilder OutFunctionConstructor
        {
            get;
            set;
        }

        public ConstructorBuilder InFunctionConstructor
        {
            get;
            set;
        }

        public TypeBuilder TypeBuilder
        {
            get;
            set;
        }

        public TypeBuilder InFunction
        {
            get;
            set;
        }

        public LFixCompiler(string name, LFixTypeDeclaration declaration, ModuleBuilder module, TypeBuilder utilityClass, MethodBuilder fmap, string moduleName)
        {
            this.name = name;
            this.declaration = declaration;
            this.module = module;
            this.utilityClass = utilityClass;
            this.fmap = fmap;
            this.moduleName = moduleName;
        }

        public void Compile()
        {
            TypeBuilder = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name,
                TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);

            var genericParameters = declaration.TypeParameters.Any() ? ((TypeBuilder)TypeBuilder).DefineGenericParameters(declaration.TypeParameters) : TypeBuilder.EmptyTypes;

            Cata = ((TypeBuilder)TypeBuilder).DefineMethod(Constants.CataMethodName,
                MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual);

            var cataReturnType = Cata.DefineGenericParameters(Constants.CataMethodGenericParameterName)[0];

            var functorClass = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, cataReturnType, genericParameters);

            Cata.SetParameters(typeof(IFunction<,>).MakeGenericType(functorClass, cataReturnType));
            Cata.SetReturnType(cataReturnType);

            CompileCataFunction();

            CompileCataFunction1();

            CompileOutClass();

            CompileOut();

            CompileOutFunction();

            CompileIn();

            CompileInFunction();

            TypeBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(FiniteAttribute).GetConstructors()[0],
                new object[] 
                {
                    declaration.ConstructorFunctionName,
                    declaration.DestructorFunctionName,
                    declaration.CataFunctionName,
                    InFunction
                }));
        }

        private void CompileOut()
        {
            Out = utilityClass.DefineMethod(Constants.OutMethodName, MethodAttributes.Public | MethodAttributes.Static);

            var genericParameters = declaration.TypeParameters.Any() ? Out.DefineGenericParameters(declaration.TypeParameters) : TypeBuilder.EmptyTypes;

            var returnType = declaration.TypeParameters.Any() ? TypeBuilder.MakeGenericType(genericParameters.ToArray()) : TypeBuilder;

            var fLeastFixedPoint = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, returnType, genericParameters);

            Out.SetParameters(fLeastFixedPoint);

            Out.SetReturnType(returnType);

            Out.SetCustomAttribute(new CustomAttributeBuilder(typeof(ExtensionAttribute).GetConstructors()[0], new object[0]));

            var outBody = Out.GetILGenerator();
            outBody.Emit(OpCodes.Ldarg_0);
            outBody.Emit(OpCodes.Newobj, genericParameters.Any()
                ? TypeBuilder.GetConstructor(
                    OutClass.MakeGenericType(genericParameters),
                    OutClassConstructor)
                : OutClassConstructor);
            outBody.Emit(OpCodes.Ret);
        }

        private void CompileOutClass()
        {
            OutClass = utilityClass.DefineNestedType(Constants.OutClassName,
               TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
               null, new[] { TypeBuilder });

            var genericParameters = declaration.TypeParameters.Any() ? OutClass.DefineGenericParameters(declaration.TypeParameters) : TypeBuilder.EmptyTypes;

            var fLeastFixedPoint = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, TypeBuilder, genericParameters);

            var predField = OutClass.DefineField(Constants.OutClassPredFieldName, fLeastFixedPoint, FieldAttributes.Private);

            OutClassConstructor = OutClass.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { fLeastFixedPoint });

            var outCtorBody = OutClassConstructor.GetILGenerator();
            outCtorBody.Emit(OpCodes.Ldarg_0);
            outCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            outCtorBody.Emit(OpCodes.Ldarg_0);
            outCtorBody.Emit(OpCodes.Ldarg_1);
            outCtorBody.Emit(OpCodes.Stfld, predField);
            outCtorBody.Emit(OpCodes.Ret);

            var cata = OutClass.DefineMethod(Constants.CataMethodName, MethodAttributes.Public | MethodAttributes.Virtual);

            var genericParameter = cata.DefineGenericParameters(Constants.CataMethodGenericParameterName)[0];

            var fGenericParameter = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, genericParameter, genericParameters);

            cata.SetParameters(typeof(IFunction<,>).MakeGenericType(fGenericParameter, genericParameter));
            cata.SetReturnType(genericParameter);

            var cataBody = cata.GetILGenerator();
            cataBody.Emit(OpCodes.Ldarg_1);
            cataBody.Emit(OpCodes.Ldarg_1);
            cataBody.Emit(OpCodes.Newobj, TypeBuilder.GetConstructor(CataFunction.MakeGenericType(new[] { genericParameter }.Concat(genericParameters).ToArray()),
                CataFunctionConstructor));
            cataBody.Emit(OpCodes.Call, fmap.MakeGenericMethod(new Type[] { TypeBuilder, genericParameter }.Concat(genericParameters).ToArray()));
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
            CataFunction1 = utilityClass.DefineNestedType(Constants.CataFunction1ClassName,
                           TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
                           null, TypeBuilder.EmptyTypes);

            var genericParameters = CataFunction1.DefineGenericParameters(new[] { Constants.CataFunction1ClassGenericParameterName }.Concat(declaration.TypeParameters).ToArray());

            var fCataClassGenericParameter = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, genericParameters[0], genericParameters.Skip(1).ToArray());

            var inputParameter = declaration.TypeParameters.Any() ? TypeBuilder.MakeGenericType(genericParameters.Skip(1).ToArray()) : TypeBuilder;

            var algebraType = typeof(IFunction<,>).MakeGenericType(fCataClassGenericParameter, genericParameters[0]);
            var initialMorphismType = typeof(IFunction<,>).MakeGenericType(inputParameter, genericParameters[0]);

            CataFunction1.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(algebraType, initialMorphismType));

            CataFunction1Constructor = CataFunction1.DefineConstructor(MethodAttributes.Public,
                CallingConventions.Standard, TypeBuilder.EmptyTypes);

            var cataCtorBody = CataFunction1Constructor.GetILGenerator();
            cataCtorBody.Emit(OpCodes.Ldarg_0);
            cataCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            cataCtorBody.Emit(OpCodes.Ret);

            var call = CataFunction1.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                initialMorphismType, new Type[] { algebraType });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Newobj, genericParameters.Any()
                ? TypeBuilder.GetConstructor(
                    CataFunction.MakeGenericType(genericParameters),
                    CataFunctionConstructor)
                : CataFunctionConstructor);
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileCataFunction()
        {
            CataFunction = utilityClass.DefineNestedType(Constants.CataFunctionClassName,
                           TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
                           null, TypeBuilder.EmptyTypes);

            var genericParameters = CataFunction.DefineGenericParameters(new[] { Constants.CataFunctionClassGenericParameterName }.Concat(declaration.TypeParameters).ToArray());

            var inputParameter = declaration.TypeParameters.Any() ? TypeBuilder.MakeGenericType(genericParameters.Skip(1).ToArray()) : TypeBuilder;

            CataFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(inputParameter, genericParameters[0]));

            var fSeedType = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, genericParameters[0], genericParameters);

            var algebra = typeof(IFunction<,>).MakeGenericType(fSeedType, genericParameters[0]);

            var seedField = CataFunction.DefineField(Constants.CataFunctionClassSeedFieldName, algebra, FieldAttributes.Private);

            CataFunctionConstructor = CataFunction.DefineConstructor(MethodAttributes.Public,
                CallingConventions.Standard, new[] { algebra });

            var cataCtorBody = CataFunctionConstructor.GetILGenerator();
            cataCtorBody.Emit(OpCodes.Ldarg_0);
            cataCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            cataCtorBody.Emit(OpCodes.Ldarg_0);
            cataCtorBody.Emit(OpCodes.Ldarg_1);
            cataCtorBody.Emit(OpCodes.Stfld, seedField);
            cataCtorBody.Emit(OpCodes.Ret);

            var call = CataFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                genericParameters[0], new Type[] { inputParameter });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Ldarg_0);
            callBody.Emit(OpCodes.Ldfld, seedField);
            callBody.Emit(OpCodes.Callvirt, genericParameters.Skip(1).Any()
                ? TypeBuilder.GetMethod(
                    TypeBuilder.MakeGenericType(genericParameters.Skip(1).ToArray()),
                    Cata).MakeGenericMethod(genericParameters[0])
                : Cata.MakeGenericMethod(genericParameters[0]));
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileIn()
        {
            In = utilityClass.DefineMethod(Constants.InMethodName, MethodAttributes.Public | MethodAttributes.Static);

            var genericParameters = declaration.TypeParameters.Any() ? In.DefineGenericParameters(declaration.TypeParameters) : TypeBuilder.EmptyTypes;

            var inputParameter = declaration.TypeParameters.Any() ? TypeBuilder.MakeGenericType(genericParameters.ToArray()) : TypeBuilder;

            In.SetReturnType(FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, inputParameter, genericParameters));

            In.SetParameters(inputParameter);

            In.SetCustomAttribute(new CustomAttributeBuilder(typeof(ExtensionAttribute).GetConstructors()[0], new object[0]));

            var inBody = In.GetILGenerator();

            var fLeastFixedPoint = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, inputParameter, genericParameters);

            inBody.Emit(OpCodes.Ldarg_0);
            inBody.Emit(OpCodes.Newobj, declaration.TypeParameters.Any()
                ? TypeBuilder.GetConstructor(
                    OutFunction.MakeGenericType(genericParameters),
                    OutFunctionConstructor)
                : OutFunctionConstructor);
            inBody.Emit(OpCodes.Call, fmap.MakeGenericMethod(new Type[] { fLeastFixedPoint, inputParameter }.Concat(genericParameters).ToArray()));
            inBody.Emit(OpCodes.Callvirt, declaration.TypeParameters.Any()
                ? TypeBuilder.GetMethod(inputParameter, Cata).MakeGenericMethod(fLeastFixedPoint)
                : Cata.MakeGenericMethod(fLeastFixedPoint));
            inBody.Emit(OpCodes.Ret);
        }

        private void CompileOutFunction()
        {
            OutFunction = utilityClass.DefineNestedType(Constants.OutFunctionClassName,
               TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic);

            var genericParameters = declaration.TypeParameters.Any() ? OutFunction.DefineGenericParameters(declaration.TypeParameters) : TypeBuilder.EmptyTypes;

            var returnType = declaration.TypeParameters.Any() ? TypeBuilder.MakeGenericType(genericParameters.ToArray()) : TypeBuilder;

            var fLeastFixedPoint = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, returnType, genericParameters);

            OutFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(fLeastFixedPoint, returnType));

            OutFunctionConstructor = OutFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, TypeBuilder.EmptyTypes);
            var outCtorBody = OutFunctionConstructor.GetILGenerator();
            outCtorBody.Emit(OpCodes.Ldarg_0);
            outCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            outCtorBody.Emit(OpCodes.Ret);

            var call = OutFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                returnType, new Type[] { fLeastFixedPoint });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Call, declaration.TypeParameters.Any() ? Out.MakeGenericMethod(genericParameters) : Out);
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileInFunction()
        {
            InFunction = utilityClass.DefineNestedType(Constants.InFunctionClassName,
               TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic);

            var genericParameters = declaration.TypeParameters.Any() ? InFunction.DefineGenericParameters(declaration.TypeParameters) : TypeBuilder.EmptyTypes;

            var inputParameter = declaration.TypeParameters.Any() ? TypeBuilder.MakeGenericType(genericParameters.ToArray()) : TypeBuilder;

            var fLeastFixedPoint = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, inputParameter, genericParameters);

            InFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(inputParameter, fLeastFixedPoint));

            InFunctionConstructor = InFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, TypeBuilder.EmptyTypes);
            var inCtorBody = InFunctionConstructor.GetILGenerator();
            inCtorBody.Emit(OpCodes.Ldarg_0);
            inCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            inCtorBody.Emit(OpCodes.Ret);

            var call = InFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                fLeastFixedPoint, new Type[] { inputParameter });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Call, declaration.TypeParameters.Any() ? In.MakeGenericMethod(genericParameters) : In);
            callBody.Emit(OpCodes.Ret);
        }
    }
}
