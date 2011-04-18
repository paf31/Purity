using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Modules;
using System.Reflection;
using Purity.Compiler.Parser;
using System.Reflection.Emit;
using Purity.Compiler.Extensions;
using Purity.Compiler.Helpers;
using Purity.Compiler.Functors;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker;
using Purity.Compiler.Types;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Typechecker.Helpers;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Data;
using Purity.Core;

namespace Purity.Compiler
{
    public class PurityCompiler
    {
        private readonly ModuleBuilder module;
        private readonly TypeBuilder dataClass;
        private readonly string moduleName;

        public PurityCompiler(string moduleName, ModuleBuilder module, TypeBuilder dataClass)
        {
            this.module = module;
            this.dataClass = dataClass;
            this.moduleName = moduleName;
        }

        public void CloseTypes()
        {
            foreach (var type in module.GetTypes().OfType<TypeBuilder>())
            {
                type.CreateType();
            }
        }

        public void Compile(string program)
        {
            var parseResult = ModuleParser.ParseModule(program);

            if (parseResult == null)
            {
                throw new CompilerException(ErrorMessages.UnableToParse);
            }

            Compile(parseResult.Output);
        }

        public void Compile(Modules.Module moduleDefinition)
        {
            foreach (var element in moduleDefinition.Elements)
            {
                string declarationName = null;

                try
                {
                    switch (element.ElementType)
                    {
                        case ProgramElementType.Functor:
                            {
                                declarationName = element.Functor.Name;
                                var functor = element.Functor.Value;
                                Container.Add(declarationName, functor);
                                break;
                            }
                        case ProgramElementType.Type:
                            {
                                declarationName = element.Type.Name;
                                var type = element.Type.Value;
                                TypeCompiler.Compile(type, dataClass, module, moduleName, declarationName);
                                Container.Add(declarationName, type);
                              break;
                            }
                        case ProgramElementType.Data:
                            {
                                declarationName = element.Data.Name;
                                var data = element.Data.Value;
                                Compile(declarationName, data);
                                break;
                            }
                    }
                }
                catch (CompilerException ex)
                {
                    throw new CompilerException(string.Format(ErrorMessages.ErrorInDeclaration, declarationName, ex.Message), ex);
                }
            }
        }

        public DataInfo Compile(string declarationName, string data)
        {
            var parseResult = DataParser.ParseData(data);

            if (parseResult == null)
            {
                throw new CompilerException(ErrorMessages.UnableToParse);
            }

            return Compile(declarationName, new DataDeclaration(parseResult.Output));
        }

        public DataInfo Compile(string declarationName, DataDeclaration data)
        {
            var typedExpression = Checker.CreateTypedExpression(data);
            typedExpression.AcceptVisitor(new AbstractionElimination());

            var collector = new TypeParameterCollector();
            data.Type.AcceptVisitor(collector);
            data.TypeParameters = collector.Parameters.ToArray();

            return MethodCompiler.Compile(declarationName, dataClass, typedExpression, data);
        }
    }
}
