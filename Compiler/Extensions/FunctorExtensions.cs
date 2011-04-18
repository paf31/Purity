using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Functors;
using Purity.Compiler.Helpers;
using System.Reflection.Emit;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Modules;
using System.Reflection;
using Purity.Core;

namespace Purity.Compiler.Extensions
{
    public static class FunctorExtensions
    {
        public static MethodBuilder Compile(this IFunctor functor, TypeBuilder utilityClass, string[] genericParameters)
        {
            var genericParameterNames = new[] 
            { 
                Constants.FMapMethodInputParameterName, 
                Constants.FMapMethodOutputParameterName 
            }.Concat(genericParameters).ToArray();

            var fmap = utilityClass.DefineMethod(Constants.FMapMethodName, MethodAttributes.Public | MethodAttributes.Static);
            var fmapParameters = fmap.DefineGenericParameters(genericParameterNames);

            fmap.SetReturnType(typeof(IFunction<,>).MakeGenericType(fmapParameters.Take(2).Select(t => new FunctorTypeMapper(t, fmapParameters).Map(functor)).ToArray()));

            fmap.SetParameters(typeof(IFunction<,>).MakeGenericType(fmapParameters.Take(2).ToArray()));

            var fmapBody = fmap.GetILGenerator();

            functor.AcceptVisitor(new FmapCompiler(fmapBody, fmapParameters));

            fmapBody.Emit(OpCodes.Ret);

            return fmap;
        }
    }
}
