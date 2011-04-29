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

namespace Purity.Compiler.Helpers
{
    public class BoxedTypeCreator
    {
        private readonly ModuleBuilder module;
        private readonly string moduleName;
        private readonly IType type;
        private readonly string name;
        private readonly string[] typeParameters;
        private FieldBuilder field;
        private ConstructorBuilder constructor;

        public TypeBuilder TypeBuilder
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

        public BoxedTypeCreator(ModuleBuilder module, string moduleName, IType type, string name, string[] typeParameters)
        {
            this.module = module;
            this.moduleName = moduleName;
            this.type = type;
            this.name = name;
            this.typeParameters = typeParameters;
        }

        public Type Compile()
        {
            TypeBuilder = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed);

            TypeBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(ExportAttribute).GetConstructors()[0], new object[0]));

            var genericParameters = typeParameters.Any() ? TypeBuilder.DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            var containedType = new TypeConverter(genericParameters).Convert(type);

            field = TypeBuilder.DefineField(Constants.BoxedTypeValueFieldName, containedType, FieldAttributes.Public);

            constructor = TypeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { containedType });

            var cataCtorBody = constructor.GetILGenerator();
            cataCtorBody.Emit(OpCodes.Ldarg_0);
            cataCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            cataCtorBody.Emit(OpCodes.Ldarg_0);
            cataCtorBody.Emit(OpCodes.Ldarg_1);
            cataCtorBody.Emit(OpCodes.Stfld, field);
            cataCtorBody.Emit(OpCodes.Ret);

            var methodsType = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name + Constants.MethodsSuffix,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract);

            CompileBoxFunction(type, TypeBuilder, methodsType, moduleName, name, typeParameters);
            CompileUnboxFunction(type, TypeBuilder, methodsType, moduleName, name, typeParameters);

            return TypeBuilder;
        }

        private void CompileBoxFunction(IType type, TypeBuilder typeBuilder, TypeBuilder methodsType, string moduleName, string name, string[] typeParameters)
        {
            BoxFunction = methodsType.DefineNestedType(Constants.BoxFunctionClassName,
               TypeAttributes.NestedPublic | TypeAttributes.Sealed | TypeAttributes.Class,
               null, Type.EmptyTypes);

            var genericParameters = typeParameters.Any() ? BoxFunction.DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            var containedType = new TypeConverter(genericParameters).Convert(type);

            BoxFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(containedType, typeBuilder));

            BoxFunctionConstructor = BoxFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var ctorBody = BoxFunctionConstructor.GetILGenerator();
            ctorBody.Emit(OpCodes.Ldarg_0);
            ctorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            ctorBody.Emit(OpCodes.Ret);

            var call = BoxFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                typeBuilder, new Type[] { containedType });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Newobj, typeParameters.Any()
                ? TypeBuilder.GetConstructor(typeBuilder.MakeGenericType(genericParameters), constructor)
                : constructor);
            callBody.Emit(OpCodes.Ret);
        }

        private void CompileUnboxFunction(IType type, TypeBuilder typeBuilder, TypeBuilder methodsType, string moduleName, string name, string[] typeParameters)
        {
            UnboxFunction = methodsType.DefineNestedType(Constants.UnboxFunctionClassName,
               TypeAttributes.NestedPublic | TypeAttributes.Sealed | TypeAttributes.Class,
               null, Type.EmptyTypes);

            var genericParameters = typeParameters.Any() ? UnboxFunction.DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            var containedType = new TypeConverter(genericParameters).Convert(type);

            UnboxFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(typeBuilder, containedType));

            UnboxFunctionConstructor = UnboxFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var ctorBody = UnboxFunctionConstructor.GetILGenerator();
            ctorBody.Emit(OpCodes.Ldarg_0);
            ctorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            ctorBody.Emit(OpCodes.Ret);

            var call = UnboxFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                containedType, new Type[] { typeBuilder });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Ldfld, typeParameters.Any()
                ? TypeBuilder.GetField(typeBuilder.MakeGenericType(genericParameters), field)
                : field);
            callBody.Emit(OpCodes.Ret);
        }
    }
}
