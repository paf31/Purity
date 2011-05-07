using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Data;
using System.Reflection.Emit;
using Purity.Compiler.Interfaces;
using System.Reflection;
using Purity.Core;
using Purity.Core.Attributes;
using Purity.Compiler.TypeDeclarations;

namespace Purity.Compiler.Helpers
{
    public class BoxedTypeCompiler
    {
        private readonly string name;
        private readonly BoxedTypeDeclaration declaration;
        private readonly ModuleBuilder module;
        private readonly string moduleName;
        private FieldBuilder field;
        private ConstructorBuilder constructor;
        private readonly IRuntimeContainer runtimeContainer;

        public TypeBuilder TypeBuilder
        {
            get;
            set;
        }

        public TypeBuilder MethodsType
        {
            get;
            set;
        }

        public ConstructorBuilder BoxFunctionConstructor
        {
            get;
            set;
        }

        public ConstructorBuilder UnboxFunctionConstructor
        {
            get;
            set;
        }

        public TypeBuilder BoxFunction
        {
            get;
            set;
        }

        public TypeBuilder UnboxFunction
        {
            get;
            set;
        }

        public BoxedTypeCompiler(string name, BoxedTypeDeclaration declaration, ModuleBuilder module, 
            string moduleName, IRuntimeContainer runtimeContainer)
        {
            this.name = name;
            this.declaration = declaration;
            this.module = module;
            this.moduleName = moduleName;
            this.runtimeContainer = runtimeContainer;
        }

        public Type Compile()
        {
            TypeBuilder = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed);

            var genericParameters = declaration.TypeParameters.Any() ? TypeBuilder.DefineGenericParameters(declaration.TypeParameters) : Type.EmptyTypes;

            var containedType = new TypeConverter(runtimeContainer, genericParameters).Convert(declaration.Type);

            field = TypeBuilder.DefineField(Constants.BoxedTypeValueFieldName, containedType, FieldAttributes.Public);

            constructor = TypeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { containedType });

            var cataCtorBody = constructor.GetILGenerator();
            cataCtorBody.Emit(OpCodes.Ldarg_0);
            cataCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            cataCtorBody.Emit(OpCodes.Ldarg_0);
            cataCtorBody.Emit(OpCodes.Ldarg_1);
            cataCtorBody.Emit(OpCodes.Stfld, field);
            cataCtorBody.Emit(OpCodes.Ret);

            MethodsType = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name + Constants.MethodsSuffix,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract);

            CompileBoxFunction();
            CompileUnboxFunction();

            TypeBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(SynonymAttribute).GetConstructors()[0],
                new object[] 
                {
                    declaration.ConstructorFunctionName,
                    declaration.DestructorFunctionName,
                    UnboxFunction
                }));

            return TypeBuilder;
        }

        private void CompileBoxFunction()
        {
            BoxFunction = MethodsType.DefineNestedType(Constants.BoxFunctionClassName,
               TypeAttributes.NestedPublic | TypeAttributes.Sealed | TypeAttributes.Class,
               null, Type.EmptyTypes);

            var genericParameters = declaration.TypeParameters.Any() ? BoxFunction.DefineGenericParameters(declaration.TypeParameters) : Type.EmptyTypes;

            var containedType = new TypeConverter(runtimeContainer, genericParameters).Convert(declaration.Type);

            BoxFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(containedType, TypeBuilder));

            BoxFunctionConstructor = BoxFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var ctorBody = BoxFunctionConstructor.GetILGenerator();
            ctorBody.Emit(OpCodes.Ldarg_0);
            ctorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            ctorBody.Emit(OpCodes.Ret);

            var call = BoxFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                TypeBuilder, new Type[] { containedType });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Newobj, declaration.TypeParameters.Any()
                ? TypeBuilder.GetConstructor(TypeBuilder.MakeGenericType(genericParameters), constructor)
                : constructor);
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileUnboxFunction()
        {
            UnboxFunction = MethodsType.DefineNestedType(Constants.UnboxFunctionClassName,
               TypeAttributes.NestedPublic | TypeAttributes.Sealed | TypeAttributes.Class,
               null, Type.EmptyTypes);

            var genericParameters = declaration.TypeParameters.Any() ? UnboxFunction.DefineGenericParameters(declaration.TypeParameters) : Type.EmptyTypes;

            var containedType = new TypeConverter(runtimeContainer, genericParameters).Convert(declaration.Type);

            UnboxFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(TypeBuilder, containedType));

            UnboxFunctionConstructor = UnboxFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var ctorBody = UnboxFunctionConstructor.GetILGenerator();
            ctorBody.Emit(OpCodes.Ldarg_0);
            ctorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            ctorBody.Emit(OpCodes.Ret);

            var call = UnboxFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                containedType, new Type[] { TypeBuilder });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Ldfld, declaration.TypeParameters.Any()
                ? TypeBuilder.GetField(TypeBuilder.MakeGenericType(genericParameters), field)
                : field);
            callBody.Emit(OpCodes.Ret);
        }
    }
}
