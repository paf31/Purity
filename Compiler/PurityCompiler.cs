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
        public string Program
        {
            get;
            set;
        }

        public PurityCompiler(string program)
        {
            Program = program;
        }

        public void Compile(string filename, string moduleName)
        {
            var parseResult = ModuleParser.ParseModule(Program);

            if (parseResult == null)
            {
                throw new CompilerException(ErrorMessages.UnableToParse);
            }

            var name = new AssemblyName(moduleName);
            AppDomain domain = AppDomain.CurrentDomain;
            var assembly = domain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save);
            var module = assembly.DefineDynamicModule(moduleName, filename);

            var dataClass = module.DefineType(moduleName + '.' + Constants.DataClassName,
                TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);

            foreach (var element in parseResult.Output.Elements)
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
                                var compiler = new TypeCompiler(module, moduleName, declarationName);
                                type.AcceptVisitor(compiler);

                                Container.Add(declarationName, type);
                                TypeContainer.Add(declarationName, compiler.Result);
                                break;
                            }
                        case ProgramElementType.Data:
                            {
                                declarationName = element.Data.Name;
                                var data = element.Data.Value;
                                var typedExpression = new Checker(data).CreateTypedExpression();
                                typedExpression.AcceptVisitor(new AbstractionElimination());

                                DataCompiler.CompileMethod(declarationName, dataClass, typedExpression, data);
                                break;
                            }
                    }
                }
                catch (CompilerException ex)
                {
                    throw new CompilerException(string.Format(ErrorMessages.ErrorInDeclaration, declarationName, ex.Message), ex);
                }
            }

            foreach (var type in module.GetTypes().OfType<TypeBuilder>())
            {
                type.CreateType();
            }

            assembly.Save(filename);
        }
    }
}
