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
        public static MethodBuilder Compile(this IFunctor functor, TypeBuilder utilityClass)
        {
            var fmap = utilityClass.DefineMethod(Constants.FMapMethodName, MethodAttributes.Public | MethodAttributes.Static);
            var fmapParameters = fmap.DefineGenericParameters(Constants.FMapMethodInputParameterName, Constants.FMapMethodOutputParameterName);

            fmap.SetReturnType(typeof(IFunction<,>).MakeGenericType(fmapParameters.Select(t => new FunctorTypeMapper(t).Map(functor)).ToArray()));

            fmap.SetParameters(typeof(IFunction<,>).MakeGenericType(fmapParameters));

            var fmapBody = fmap.GetILGenerator();

            functor.AcceptVisitor(new FmapCompiler(fmapBody, fmapParameters[0], fmapParameters[1]));

            fmapBody.Emit(OpCodes.Ret);

            return fmap;
        }
    }
}
