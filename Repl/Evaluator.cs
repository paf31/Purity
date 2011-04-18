using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using Purity.Compiler.Data;
using Purity.Compiler;
using Purity.Compiler.Exceptions;
using Repl.Helpers;

namespace Repl
{
    public static class Evaluator
    {
        public static string Evaluate(ModuleBuilder module, string moduleName, string expression, string name)
        {
            foreach (var type in module.GetTypes().OfType<TypeBuilder>())
            {
                type.CreateType();
            }

            var evalClass = module.DefineType(name,
                TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);

            var compiler = new PurityCompiler(moduleName, module, evalClass);

            DataInfo dataInfo = compiler.Compile(name, expression);

            var createdType = evalClass.CreateType();
            var method = createdType.GetMethod(name, Type.EmptyTypes);

            if (method == null)
            {
                throw new Exception("Cannot evaluate expression.");
            }
            else
            {
                var decl = Container.ResolveValue(name);

                if (decl.TypeParameters.Any())
                {
                    throw new Exception("Cannot evaluate expression.");
                }
                else
                {
                    var result = method.Invoke(null, new object[0]);
                    return PrintData.Print(result, decl.Type, 10);
                }
            }
        }
    }
}
