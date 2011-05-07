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
using Purity.Compiler.Interfaces;

namespace Repl
{
    public class Evaluator
    {
        private const string EvalClassName = "Eval";

        private int index = 0;
        private readonly ModuleBuilder module;
        private readonly IMetadataContainer container;
        private readonly IRuntimeContainer runtimeContainer;

        public Evaluator(ModuleBuilder module, IMetadataContainer container, IRuntimeContainer runtimeContainer)
        {
            this.module = module;
            this.container = container;
            this.runtimeContainer = runtimeContainer;
        }

        public string Evaluate(string expression)
        {
            foreach (var type in module.GetTypes().OfType<TypeBuilder>())
            {
                type.CreateType();
            }

            string name = EvalClassName + index++;

            var evalClass = module.DefineType(name,
                TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);

            new PurityCompiler(module, evalClass, container, runtimeContainer).Compile(name, expression);

            var createdType = evalClass.CreateType();
            var method = createdType.GetMethod(name, Type.EmptyTypes);

            if (method == null)
            {
                throw new Exception("Cannot evaluate expression.");
            }
            else
            {
                var decl = container.ResolveValue(name);

                if (decl.TypeParameters.Any())
                {
                    throw new Exception("Cannot evaluate expression.");
                }
                else
                {
                    var result = method.Invoke(null, new object[0]);
                    return PrintData.Print(result, decl.Type, 10, container, runtimeContainer);
                }
            }
        }
    }
}
