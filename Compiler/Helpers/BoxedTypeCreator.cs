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
        public static BoxedTypeInfo CreateBoxedType(IType type, ModuleBuilder module, string moduleName, string name)
        {
            var boxedTypeInfo = new BoxedTypeInfo();

            var containedType = new TypeConverter(null).Convert(type);

            boxedTypeInfo.Type = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed);

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

            CompileBoxFunction(boxedTypeInfo, methodsType, moduleName, name, containedType);
            CompileUnboxFunction(boxedTypeInfo, methodsType, moduleName, name, containedType);

            return boxedTypeInfo;
        }

        private static void CompileBoxFunction(BoxedTypeInfo typeInfo, TypeBuilder methodsType, string moduleName, string name, Type containedType)
        {
            typeInfo.BoxFunction = methodsType.DefineNestedType(Constants.BoxFunctionClassName,
               TypeAttributes.NestedPublic | TypeAttributes.Sealed | TypeAttributes.Class,
               null, new[] { typeof(IFunction<,>).MakeGenericType(containedType, typeInfo.Type) });

            typeInfo.BoxFunctionConstructor = typeInfo.BoxFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var ctorBody = typeInfo.BoxFunctionConstructor.GetILGenerator();
            ctorBody.Emit(OpCodes.Ldarg_0);
            ctorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            ctorBody.Emit(OpCodes.Ret);

            var call = typeInfo.BoxFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                typeInfo.Type, new Type[] { containedType });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Newobj, typeInfo.Constructor);
            callBody.Emit(OpCodes.Ret);
        }

        private static void CompileUnboxFunction(BoxedTypeInfo typeInfo, TypeBuilder methodsType, string moduleName, string name, Type containedType)
        {
            typeInfo.UnboxFunction = methodsType.DefineNestedType(Constants.UnboxFunctionClassName,
               TypeAttributes.NestedPublic | TypeAttributes.Sealed | TypeAttributes.Class,
               null, new[] { typeof(IFunction<,>).MakeGenericType(typeInfo.Type, containedType) });

            typeInfo.UnboxFunctionConstructor = typeInfo.UnboxFunction.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var ctorBody = typeInfo.UnboxFunctionConstructor.GetILGenerator();
            ctorBody.Emit(OpCodes.Ldarg_0);
            ctorBody.Emit(OpCodes.Call, typeof(object).GetConstructors()[0]);
            ctorBody.Emit(OpCodes.Ret);

            var call = typeInfo.UnboxFunction.DefineMethod(Constants.CallMethodName, MethodAttributes.Public | MethodAttributes.Virtual,
                containedType, new Type[] { typeInfo.Type });

            var callBody = call.GetILGenerator();
            callBody.Emit(OpCodes.Ldarg_1);
            callBody.Emit(OpCodes.Ldfld, typeInfo.Field);
            callBody.Emit(OpCodes.Ret);
        }
    }
}
