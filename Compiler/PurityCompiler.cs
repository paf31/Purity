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
using Purity.Compiler.Helpers;
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
        private readonly ModuleBuilder module;
        private readonly TypeBuilder dataClass;
        private readonly IMetadataContainer container;
        private readonly IRuntimeContainer runtimeContainer;
        private readonly TypeChecker typeChecker;
        private readonly ModuleImporter moduleImporter;

        public PurityCompiler(ModuleBuilder module, TypeBuilder dataClass, IMetadataContainer container,
            IRuntimeContainer runtimeContainer)
        {
            this.module = module;
            this.dataClass = dataClass;
            this.container = container;
            this.runtimeContainer = runtimeContainer;
            this.typeChecker = new TypeChecker(container);
            this.moduleImporter = new ModuleImporter(container, runtimeContainer);
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
                    case ProgramElementType.Type:
                        {
                            declarationName = element.Type.Name;
                            var type = element.Type.Value;
                            TypeCompiler.Compile(type, dataClass, module, moduleDefinitionName, 
                                declarationName, container, runtimeContainer);
                            container.Add(declarationName, type);
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
                                System.Reflection.Module module = moduleImporter.Import(moduleName);

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
            var result = typeChecker.CreateTypedExpression(data);

            data.Type = result.Item1;

            var typedExpression = result.Item2;
            typedExpression.AcceptVisitor(new AbstractionElimination(container));

            var collector = new TypeParameterCollector();
            data.Type.AcceptVisitor(collector);
            data.TypeParameters = collector.Parameters.ToArray();

            MethodCompiler.Compile(declarationName, dataClass, typedExpression, data, container, runtimeContainer);
        }
    }
}
