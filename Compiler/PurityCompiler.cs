using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Purity.Compiler.Parser;
using System.Reflection.Emit;
using Purity.Core;
using Purity.Compiler.Data;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Modules;
using Purity.Compiler.Extensions;
using Purity.Compiler.Helpers;
using Purity.Compiler.Functors;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Types;
using Purity.Compiler.Typechecker.Helpers;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Typechecker.Utilities;
using Purity.Compiler.Utilities;

namespace Purity.Compiler
{
    public class PurityCompiler
    {
        private ModuleBuilder module;
        private TypeBuilder dataClass;

        public PurityCompiler(ModuleBuilder module, TypeBuilder dataClass)
        {
            this.module = module;
            this.dataClass = dataClass;
        }

        public void CloseTypes()
        {
            foreach (var type in module.GetTypes().OfType<TypeBuilder>())
            {
                type.CreateType();
            }
        }

        public void Compile(Modules.Module moduleDefinition)
        {
            foreach (var element in moduleDefinition.Elements)
            {
                Compile(element, moduleDefinition.Name);
            }
        }

        public void Compile(ProgramElement element, string moduleDefinitionName)
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
                            TypeCompiler.Compile(type, dataClass, module, moduleDefinitionName, declarationName);
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
                    case ProgramElementType.Import:
                        {
                            string moduleName = element.Import.ModuleName;

                            try
                            {
                                System.Reflection.Module module = ModuleImporter.Import(moduleName);

                            }
                            catch (ModuleImportException ex)
                            {
                                throw new CompilerException(ex.Message, ex);
                            }
                            break;
                        }
                }
            }
            catch (CompilerException ex)
            {
                throw new CompilerException(string.Format(ErrorMessages.ErrorInDeclaration, declarationName, ex.Message), ex);
            }
        }

        public void Compile(string declarationName, string data)
        {
            var parseResult = DataParser.ParseData(data);

            if (parseResult == null)
            {
                throw new CompilerException(ErrorMessages.UnableToParse);
            }

            Compile(declarationName, new DataDeclaration(parseResult.Output));
        }

        public void Compile(string declarationName, DataDeclaration data)
        {
            var result = TypeChecker.CreateTypedExpression(data);

            data.Type = result.Item1;

            var typedExpression = result.Item2;
            typedExpression.AcceptVisitor(new AbstractionElimination());

            var collector = new TypeParameterCollector();
            data.Type.AcceptVisitor(collector);
            data.TypeParameters = collector.Parameters.ToArray();

            MethodCompiler.Compile(declarationName, dataClass, typedExpression, data);
        }
    }
}
