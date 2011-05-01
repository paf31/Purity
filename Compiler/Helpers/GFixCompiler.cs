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
    public class GFixCompiler
    {
        private readonly string name;
        private readonly GFixTypeDeclaration declaration;
        private readonly ModuleBuilder module;
        private readonly TypeBuilder utilityClass;
        private readonly string moduleName;
        private readonly MethodBuilder fmap;
        private TypeBuilder GreatestFixedPointFunction;
        private MethodBuilder GreatestFixedPointFunctionApplyMethod;
        private MethodBuilder Ana;
        private MethodBuilder In;
        private MethodBuilder Out;
        private TypeBuilder AnaClass;
        private ConstructorBuilder AnaClassConstructor;
        private TypeBuilder AnaFunction;
        private ConstructorBuilder AnaFunctionConstructor;
        private TypeBuilder AnaFunction1;
        private ConstructorBuilder InGeneratingFunctionConstructor;
        private MethodBuilder GreatestFixedPointApplyMethod;
        private TypeBuilder OutFunction;
        private TypeBuilder InClass;

        public ConstructorBuilder InFunctionConstructor
        {
            get;
            set;
        }

        public ConstructorBuilder AnaFunction1Constructor
        {
            get;
            set;
        }

        public ConstructorBuilder OutFunctionConstructor
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

        public GFixCompiler(string name, GFixTypeDeclaration declaration, ModuleBuilder module, TypeBuilder utilityClass, MethodBuilder fmap, string moduleName)
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
            GreatestFixedPointFunction = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name + Constants.FunctionSuffix,
                 TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);

            var functionClassGenericParameters = GreatestFixedPointFunction.DefineGenericParameters(new[] { Constants.GFixFunctionClassGenericParameterName }.Concat(declaration.TypeParameters).ToArray());

            GreatestFixedPointFunctionApplyMethod = GreatestFixedPointFunction.DefineMethod(Constants.ApplyMethodName,
                 MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual);

            var underlyingType = GreatestFixedPointFunctionApplyMethod.DefineGenericParameters(Constants.ApplyMethodGenericParameterName)[0];

            var functorClass = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, underlyingType, functionClassGenericParameters.Skip(1).ToArray());

            GreatestFixedPointFunctionApplyMethod.SetParameters(underlyingType, typeof(IFunction<,>).MakeGenericType(underlyingType, functorClass));
            GreatestFixedPointFunctionApplyMethod.SetReturnType(functionClassGenericParameters[0]);

            TypeBuilder = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name,
                 TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);

            var typeGenericParameters = declaration.TypeParameters.Any() ? TypeBuilder.DefineGenericParameters(declaration.TypeParameters) : TypeBuilder.EmptyTypes;

            GreatestFixedPointApplyMethod = TypeBuilder.DefineMethod(Constants.ApplyMethodName,
                 MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual);

            var resultTypeGfix = GreatestFixedPointApplyMethod.DefineGenericParameters(Constants.GFixFunctionClassGenericParameterName)[0];

            GreatestFixedPointApplyMethod.SetParameters(GreatestFixedPointFunction.MakeGenericType(new[] { resultTypeGfix }.Concat(typeGenericParameters).ToArray()));
            GreatestFixedPointApplyMethod.SetReturnType(resultTypeGfix);

            CompileAnaClass();

            CompileAna();

            CompileAnaFunction();

            CompileAnaFunction1();

            CompileIn();

            CompileInFunction();

            CompileOut();

            CompileOutFunction();

            TypeBuilder.SetCustomAttribute(new CustomAttributeBuilder(
                typeof(InfiniteAttribute).GetConstructors()[0],
                new object[] 
                {
                    declaration.ConstructorFunctionName, 
                    declaration.DestructorFunctionName, 
                    declaration.AnaFunctionName,
                    InFunction
                }));
        }

        private void CompileAna()
        {
            Ana = utilityClass.DefineMethod(Constants.AnaMethodName, MethodAttributes.Public | MethodAttributes.Static);

            var anaGenericParameters = Ana.DefineGenericParameters(new[] { Constants.AnaMethodGenericParameterName }.Concat(declaration.TypeParameters).ToArray());

            Ana.SetReturnType(declaration.TypeParameters.Any()
                 ? TypeBuilder.MakeGenericType(anaGenericParameters.Skip(1).ToArray())
                 : TypeBuilder);

            var functorClass = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, anaGenericParameters[0], anaGenericParameters.Skip(1).ToArray());

            var generatorType = typeof(IFunction<,>).MakeGenericType(anaGenericParameters[0], functorClass);

            Ana.SetParameters(anaGenericParameters[0], generatorType);

            var anaBody = Ana.GetILGenerator();
            anaBody.Emit(OpCodes.Ldarg_0);
            anaBody.Emit(OpCodes.Ldarg_1);
            anaBody.Emit(OpCodes.Newobj, TypeBuilder.GetConstructor(AnaClass.MakeGenericType(anaGenericParameters), AnaClassConstructor));
            anaBody.Emit(OpCodes.Ret);
        }

        private void CompileAnaClass()
        {
            AnaClass = utilityClass.DefineNestedType(Constants.AnaClassName,
                            TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
                            null, TypeBuilder.EmptyTypes);

            var anaClassGenericParameters = AnaClass.DefineGenericParameters(new[] { Constants.AnaClassGenericParameterName }.Concat(declaration.TypeParameters).ToArray());

            if (declaration.TypeParameters.Any())
            {
                AnaClass.AddInterfaceImplementation(TypeBuilder.MakeGenericType(anaClassGenericParameters.Skip(1).ToArray()));
            }
            else
            {
                AnaClass.AddInterfaceImplementation(TypeBuilder);
            }

            var anaClassGeneratorType = typeof(IFunction<,>).MakeGenericType(anaClassGenericParameters[0],
                FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, anaClassGenericParameters[0], anaClassGenericParameters.Skip(1).ToArray()));

            var seedField = AnaClass.DefineField(Constants.AnaClassSeedFieldName, anaClassGenericParameters[0], FieldAttributes.Private);
            var generatorField = AnaClass.DefineField(Constants.AnaClassGeneratorFieldName, anaClassGeneratorType, FieldAttributes.Private);

            AnaClassConstructor = AnaClass.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { anaClassGenericParameters[0], anaClassGeneratorType });
            var ctorBody = AnaClassConstructor.GetILGenerator();
            ctorBody.Emit(OpCodes.Ldarg_0);
            ctorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            ctorBody.Emit(OpCodes.Ldarg_0);
            ctorBody.Emit(OpCodes.Ldarg_1);
            ctorBody.Emit(OpCodes.Stfld, seedField);
            ctorBody.Emit(OpCodes.Ldarg_0);
            ctorBody.Emit(OpCodes.Ldarg_2);
            ctorBody.Emit(OpCodes.Stfld, generatorField);
            ctorBody.Emit(OpCodes.Ret);

            var apply = AnaClass.DefineMethod(Constants.ApplyMethodName, MethodAttributes.Public | MethodAttributes.Virtual);

            var resultType = apply.DefineGenericParameters(Constants.GFixFunctionClassGenericParameterName)[0];

            var applyFunctionGenericParameters = new[] { resultType }.Concat(anaClassGenericParameters.Skip(1)).ToArray();

            apply.SetParameters(GreatestFixedPointFunction.MakeGenericType(applyFunctionGenericParameters));
            apply.SetReturnType(resultType);

            var applyMethodGenericClass = TypeBuilder.GetMethod(
               GreatestFixedPointFunction.MakeGenericType(applyFunctionGenericParameters),
               GreatestFixedPointFunctionApplyMethod);
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

            In = utilityClass.DefineMethod(Constants.InMethodName, MethodAttributes.Public | MethodAttributes.Static);

            var genericParameters = declaration.TypeParameters.Any() ? In.DefineGenericParameters(declaration.TypeParameters) : TypeBuilder.EmptyTypes;

            var resultType = declaration.TypeParameters.Any()
                ? TypeBuilder.MakeGenericType(genericParameters.ToArray())
                : TypeBuilder;

            In.SetReturnType(FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, resultType, genericParameters));

            In.SetParameters(resultType);

            In.SetCustomAttribute(new CustomAttributeBuilder(typeof(ExtensionAttribute).GetConstructors()[0], new object[0]));

            var fGreatestFixedPoint = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, resultType, genericParameters);

            var applyMethodGenericClass = declaration.TypeParameters.Any()
                ? TypeBuilder.GetMethod(
                   TypeBuilder.MakeGenericType(genericParameters),
                   GreatestFixedPointApplyMethod)
                : GreatestFixedPointApplyMethod;
            var applyMethodGenericMethod = applyMethodGenericClass.MakeGenericMethod(fGreatestFixedPoint);

            var inBody = In.GetILGenerator();
            inBody.Emit(OpCodes.Ldarg_0);
            inBody.Emit(OpCodes.Newobj, declaration.TypeParameters.Any()
                ? TypeBuilder.GetConstructor(
                   InClass.MakeGenericType(genericParameters),
                   InGeneratingFunctionConstructor)
                : InGeneratingFunctionConstructor);
            inBody.Emit(OpCodes.Callvirt, applyMethodGenericMethod);
            inBody.Emit(OpCodes.Ret);
        }

        private void CompileInGeneratingFunction()
        {
            InClass = utilityClass.DefineNestedType(Constants.InGeneratingFunctionClassName,
                TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic);

            var genericParameters = declaration.TypeParameters.Any() ? InClass.DefineGenericParameters(declaration.TypeParameters) : TypeBuilder.EmptyTypes;

            var greatestFixedPoint = declaration.TypeParameters.Any()
                ? TypeBuilder.MakeGenericType(genericParameters)
                : TypeBuilder;

            var fGreatestFixedPoint = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, greatestFixedPoint, genericParameters);

            InClass.AddInterfaceImplementation(GreatestFixedPointFunction.MakeGenericType(new[] { fGreatestFixedPoint }.Concat(genericParameters).ToArray()));

            InGeneratingFunctionConstructor = InClass.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, TypeBuilder.EmptyTypes);
            var inCtorBody = InGeneratingFunctionConstructor.GetILGenerator();
            inCtorBody.Emit(OpCodes.Ldarg_0);
            inCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            inCtorBody.Emit(OpCodes.Ret);

            var apply = InClass.DefineMethod(Constants.ApplyMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                fGreatestFixedPoint, TypeBuilder.EmptyTypes);

            var genericParameter = apply.DefineGenericParameters(Constants.ApplyMethodGenericParameterName)[0];

            var fGenericParameter = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, genericParameter, genericParameters);

            apply.SetParameters(genericParameter, typeof(IFunction<,>).MakeGenericType(genericParameter, fGenericParameter));

            var applyBody = apply.GetILGenerator();
            applyBody.Emit(OpCodes.Ldarg_2);
            applyBody.Emit(OpCodes.Newobj, TypeBuilder.GetConstructor(
               AnaFunction.MakeGenericType(new[] { genericParameter }.Concat(genericParameters).ToArray()),
               AnaFunctionConstructor));
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
            AnaFunction = utilityClass.DefineNestedType(Constants.AnaFunctionClassName,
                           TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
                           null, TypeBuilder.EmptyTypes);

            var anaClassGenericParameters = AnaFunction.DefineGenericParameters(new[] { Constants.AnaFunctionClassGenericParameterName }.Concat(declaration.TypeParameters).ToArray());

            var resultType = declaration.TypeParameters.Any()
                ? TypeBuilder.MakeGenericType(anaClassGenericParameters.Skip(1).ToArray())
                : TypeBuilder;

            AnaFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(anaClassGenericParameters[0],
                 resultType));

            var seedType = typeof(IFunction<,>).MakeGenericType(anaClassGenericParameters[0],
                FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, anaClassGenericParameters[0], anaClassGenericParameters.Skip(1).ToArray()));

            var seedField = AnaFunction.DefineField(Constants.AnaFunctionClassSeedFieldName, seedType, FieldAttributes.Private);

            AnaFunctionConstructor = AnaFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { seedType });

            var anaCtorBody = AnaFunctionConstructor.GetILGenerator();
            anaCtorBody.Emit(OpCodes.Ldarg_0);
            anaCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            anaCtorBody.Emit(OpCodes.Ldarg_0);
            anaCtorBody.Emit(OpCodes.Ldarg_1);
            anaCtorBody.Emit(OpCodes.Stfld, seedField);
            anaCtorBody.Emit(OpCodes.Ret);

            var call = AnaFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                resultType, new[] { anaClassGenericParameters[0] });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Ldarg_0);
            callBody.Emit(OpCodes.Ldfld, seedField);
            callBody.Emit(OpCodes.Call, Ana.MakeGenericMethod(anaClassGenericParameters));
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileAnaFunction1()
        {
            AnaFunction1 = utilityClass.DefineNestedType(Constants.AnaFunction1ClassName,
                           TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic,
                           null, TypeBuilder.EmptyTypes);

            var anaClassGenericParameters = AnaFunction1.DefineGenericParameters(new[] { Constants.AnaFunction1ClassGenericParameterName }.Concat(declaration.TypeParameters).ToArray());

            var resultType = declaration.TypeParameters.Any()
                ? TypeBuilder.MakeGenericType(anaClassGenericParameters.Skip(1).ToArray())
                : TypeBuilder;

            var fAnaClassGenericParameter = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, anaClassGenericParameters[0], anaClassGenericParameters.Skip(1).ToArray());

            var coalgebraType = typeof(IFunction<,>).MakeGenericType(anaClassGenericParameters[0], fAnaClassGenericParameter);
            var terminalMorphismType = typeof(IFunction<,>).MakeGenericType(anaClassGenericParameters[0], resultType);

            AnaFunction1.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(coalgebraType,
                 terminalMorphismType));

            AnaFunction1Constructor = AnaFunction1.DefineConstructor(MethodAttributes.Public,
                 CallingConventions.Standard, TypeBuilder.EmptyTypes);

            var anaCtorBody = AnaFunction1Constructor.GetILGenerator();
            anaCtorBody.Emit(OpCodes.Ldarg_0);
            anaCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            anaCtorBody.Emit(OpCodes.Ret);

            var call = AnaFunction1.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                terminalMorphismType, new Type[] { coalgebraType });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Newobj, TypeBuilder.GetConstructor(
               AnaFunction.MakeGenericType(anaClassGenericParameters),
               AnaFunctionConstructor));
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileOut()
        {
            Out = utilityClass.DefineMethod(Constants.OutMethodName, MethodAttributes.Public | MethodAttributes.Static);

            var genericParameters = declaration.TypeParameters.Any() ? Out.DefineGenericParameters(declaration.TypeParameters) : TypeBuilder.EmptyTypes;

            var returnType = declaration.TypeParameters.Any()
                ? TypeBuilder.MakeGenericType(genericParameters)
                : TypeBuilder;

            var fGreatestFixedPoint = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, returnType, genericParameters);

            Out.SetParameters(fGreatestFixedPoint);

            Out.SetReturnType(returnType);

            Out.SetCustomAttribute(new CustomAttributeBuilder(typeof(ExtensionAttribute).GetConstructors()[0], new object[0]));

            var outBody = Out.GetILGenerator();
            outBody.Emit(OpCodes.Ldarg_0);
            outBody.Emit(OpCodes.Newobj, declaration.TypeParameters.Any()
                ? TypeBuilder.GetConstructor(
                   InFunction.MakeGenericType(genericParameters),
                   InFunctionConstructor)
                : InFunctionConstructor);
            outBody.Emit(OpCodes.Call, fmap.MakeGenericMethod(new[] { returnType, fGreatestFixedPoint }.Concat(genericParameters).ToArray()));
            outBody.Emit(OpCodes.Call, Ana.MakeGenericMethod(new[] { fGreatestFixedPoint }.Concat(genericParameters).ToArray()));
            outBody.Emit(OpCodes.Ret);
        }

        private void CompileInFunction()
        {
            InFunction = utilityClass.DefineNestedType(Constants.InFunctionClassName,
                TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic);

            var genericParameters = declaration.TypeParameters.Any() ? InFunction.DefineGenericParameters(declaration.TypeParameters) : TypeBuilder.EmptyTypes;

            var fGreatestFixedPoint = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, TypeBuilder, genericParameters);

            InFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(TypeBuilder, fGreatestFixedPoint));

            InFunctionConstructor = InFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, TypeBuilder.EmptyTypes);
            var inCtorBody = InFunctionConstructor.GetILGenerator();
            inCtorBody.Emit(OpCodes.Ldarg_0);
            inCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            inCtorBody.Emit(OpCodes.Ret);

            var call = InFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                fGreatestFixedPoint, new Type[] { TypeBuilder });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Call, declaration.TypeParameters.Any()
                ? In.MakeGenericMethod(genericParameters)
                : In);
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileOutFunction()
        {
            OutFunction = utilityClass.DefineNestedType(Constants.OutFunctionClassName,
                TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.NestedPublic);

            var genericParameters = declaration.TypeParameters.Any() ? OutFunction.DefineGenericParameters(declaration.TypeParameters) : TypeBuilder.EmptyTypes;

            var fGreatestFixedPoint = FunctorTypeMapper.Map(declaration.Type, declaration.VariableName, TypeBuilder, genericParameters);

            OutFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(fGreatestFixedPoint, TypeBuilder));

            OutFunctionConstructor = OutFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, TypeBuilder.EmptyTypes);
            var outCtorBody = OutFunctionConstructor.GetILGenerator();
            outCtorBody.Emit(OpCodes.Ldarg_0);
            outCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            outCtorBody.Emit(OpCodes.Ret);

            var call = OutFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
               TypeBuilder, new Type[] { fGreatestFixedPoint });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Call, declaration.TypeParameters.Any()
                ? Out.MakeGenericMethod(genericParameters)
                : Out);
            callBody.Emit(OpCodes.Ret);
        }
    }
}
