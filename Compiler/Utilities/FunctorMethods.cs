using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Helpers;
using System.Reflection.Emit;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Modules;
using System.Reflection;
using Purity.Core;

namespace Purity.Compiler.Utilities
{
    public static class FunctorMethods
    {
        public static MethodBuilder Compile(IType functorType, string variableName, 
            TypeBuilder utilityClass, string[] genericParameters, IRuntimeContainer runtimeContainer)
        {
            var genericParameterNames = new[] 
            { 
                Constants.FMapMethodInputParameterName, 
                Constants.FMapMethodOutputParameterName 
            }.Concat(genericParameters).ToArray();

            var fmap = utilityClass.DefineMethod(Constants.FMapMethodName, MethodAttributes.Public | MethodAttributes.Static);
            var fmapParameters = fmap.DefineGenericParameters(genericParameterNames);

            fmap.SetReturnType(typeof(IFunction<,>).MakeGenericType(fmapParameters.Take(2).Select(t => FunctorTypeMapper.Map(functorType, variableName, t, fmapParameters, runtimeContainer)).ToArray()));

            fmap.SetParameters(typeof(IFunction<,>).MakeGenericType(fmapParameters.Take(2).ToArray()));

            var fmapBody = fmap.GetILGenerator();

            functorType.AcceptVisitor(new FmapCompiler(fmapBody, variableName, fmapParameters, runtimeContainer));

            fmapBody.Emit(OpCodes.Ret);

            return fmap;
        }
    }
}
