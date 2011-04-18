using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Data;
using System.Reflection.Emit;
using Purity.Compiler.Interfaces;
using System.Reflection;
using Purity.Core;

namespace Purity.Compiler.Helpers
{
    public static class BoxedTypeCreator
    {
        public static BoxedTypeInfo CreateBoxedType(IType type, ModuleBuilder module, string moduleName, string name, string[] typeParameters)
        {
            var boxedTypeInfo = new BoxedTypeInfo();

            boxedTypeInfo.Type = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed);

            var genericParameters = typeParameters.Any() ? boxedTypeInfo.Type.DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            var containedType = new TypeConverter(genericParameters).Convert(type);

            boxedTypeInfo.Field = boxedTypeInfo.Type.DefineField(Constants.BoxedTypeValueFieldName, containedType, FieldAttributes.Public);

            boxedTypeInfo.Constructor = boxedTypeInfo.Type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { containedType });

            var cataCtorBody = boxedTypeInfo.Constructor.GetILGenerator();
            cataCtorBody.Emit(OpCodes.Ldarg_0);
            cataCtorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            cataCtorBody.Emit(OpCodes.Ldarg_0);
            cataCtorBody.Emit(OpCodes.Ldarg_1);
            cataCtorBody.Emit(OpCodes.Stfld, boxedTypeInfo.Field);
            cataCtorBody.Emit(OpCodes.Ret);

            var methodsType = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name + Constants.MethodsSuffix,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract);

            CompileBoxFunction(type, boxedTypeInfo, methodsType, moduleName, name, typeParameters);
            CompileUnboxFunction(type, boxedTypeInfo, methodsType, moduleName, name, typeParameters);

            return boxedTypeInfo;
        }

        private static void CompileBoxFunction(IType type, BoxedTypeInfo typeInfo, TypeBuilder methodsType, string moduleName, string name, string[] typeParameters)
        {
            var boxFunction = methodsType.DefineNestedType(Constants.BoxFunctionClassName,
               TypeAttributes.NestedPublic | TypeAttributes.Sealed | TypeAttributes.Class,
               null, Type.EmptyTypes);

            var genericParameters = typeParameters.Any() ? boxFunction.DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            var containedType = new TypeConverter(genericParameters).Convert(type);

            boxFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(containedType, typeInfo.Type));

            typeInfo.BoxFunction = boxFunction;

            typeInfo.BoxFunctionConstructor = boxFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var ctorBody = typeInfo.BoxFunctionConstructor.GetILGenerator();
            ctorBody.Emit(OpCodes.Ldarg_0);
            ctorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            ctorBody.Emit(OpCodes.Ret);

            var call = boxFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                typeInfo.Type, new Type[] { containedType });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Newobj, typeParameters.Any()
                ? TypeBuilder.GetConstructor(typeInfo.Type.MakeGenericType(genericParameters), typeInfo.Constructor)
                : typeInfo.Constructor);
            callBody.Emit(OpCodes.Ret);
        }

        private static void CompileUnboxFunction(IType type, BoxedTypeInfo typeInfo, TypeBuilder methodsType, string moduleName, string name, string[] typeParameters)
        {
            var unboxFunction = methodsType.DefineNestedType(Constants.UnboxFunctionClassName,
               TypeAttributes.NestedPublic | TypeAttributes.Sealed | TypeAttributes.Class,
               null, Type.EmptyTypes);

            var genericParameters = typeParameters.Any() ? unboxFunction.DefineGenericParameters(typeParameters) : Type.EmptyTypes;

            var containedType = new TypeConverter(genericParameters).Convert(type);

            unboxFunction.AddInterfaceImplementation(typeof(IFunction<,>).MakeGenericType(typeInfo.Type, containedType));

            typeInfo.UnboxFunction = unboxFunction;

            typeInfo.UnboxFunctionConstructor = unboxFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var ctorBody = typeInfo.UnboxFunctionConstructor.GetILGenerator();
            ctorBody.Emit(OpCodes.Ldarg_0);
            ctorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            ctorBody.Emit(OpCodes.Ret);

            var call = unboxFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                containedType, new Type[] { typeInfo.Type });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Ldfld, typeParameters.Any() 
                ? TypeBuilder.GetField(typeInfo.Type.MakeGenericType(genericParameters), typeInfo.Field)
                : typeInfo.Field);
            callBody.Emit(OpCodes.Ret);
        }
    }
}
