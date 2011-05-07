using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler;
using System.Reflection;
using System.Reflection.Emit;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Modules;
using Repl.Helpers;
using Purity.Core.Types;
using Purity.Compiler.Data;
using Purity.Compiler.Parser;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Implementation;
using Purity.Compiler.Utilities;

namespace Repl
{
    public class Program
    {
        private const string Prompt = "Purity> ";
        private const string AssemblyName = "Repl";
        private const char CommandIntroduction = ':';
        private const char ResetCommand = 'r';
        private const char ClearCommand = 'c';
        private const char PrintDeclarationsCommand = 'd';
        private const char PrintTypeCommand = 't';
        private const char EvaluateCommand = 'e';

        private static readonly IMetadataContainer container = new MetadataContainer();
        private static readonly IRuntimeContainer runtimeContainer = new RuntimeContainer();

        public static void Main(string[] args)
        {
            int index = 0;
            int assemblyIndex = 0;

            var name = new AssemblyName(AssemblyName + assemblyIndex++);
            AppDomain domain = AppDomain.CurrentDomain;
            var assembly = domain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule(AssemblyName);

            ModuleImporter moduleImporter = new ModuleImporter(container, runtimeContainer);

            Evaluator evaluator = new Evaluator(module, container, runtimeContainer);

            Console.TreatControlCAsInput = false;

            while (true)
            {
                try
                {
                    Console.Write(Prompt);

                    string line = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (line[0] == CommandIntroduction)
                        {
                            switch (line[1])
                            {
                                case ClearCommand:
                                    // Clear console
                                    Console.Clear();
                                    break;
                                case ResetCommand:
                                    // Clear all data
                                    container.Clear();
                                    runtimeContainer.Clear();
                                    name = new AssemblyName(AssemblyName + assemblyIndex++);
                                    assembly = domain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
                                    module = assembly.DefineDynamicModule(AssemblyName);
                                    break;
                                case PrintDeclarationsCommand:
                                    // Print definitions
                                    PrintDeclarations();
                                    break;
                                case PrintTypeCommand:
                                    // Print type
                                    PrintType(line.Substring(2).Trim());
                                    break;
                                case EvaluateCommand:
                                    // Evaluate
                                    string result = evaluator.Evaluate(line.Substring(2).Trim());
                                    PrintResult(result);
                                    break;
                                default:
                                    PrintError("Unknown command.");
                                    break;
                            }
                        }
                        else
                        {
                            var dataClass = module.DefineType(Constants.DataClassName + index++,
                                TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);

                            var compiler = new PurityCompiler(module, dataClass, container, runtimeContainer);

                            var parseResult = ModuleParser.ParseProgramElement(line);

                            if (parseResult == null)
                            {
                                throw new CompilerException(ErrorMessages.UnableToParse);
                            }

                            try
                            {
                                compiler.Compile(parseResult.Output, module.Name);
                            }
                            catch (CompilerException ex)
                            {
                                PrintError(ex.Message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    PrintError(ex.Message);
                }
            }
        }

        private static void PrintResult(string resultText)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(resultText);
            Console.ResetColor();
        }

        private static void PrintError(string errorText)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(errorText);
            Console.ResetColor();
        }

        private static void PrintType(string name)
        {
            var decl = container.ResolveValue(name);
            Console.WriteLine(Helpers.PrintType.Print(decl.Type));
        }

        private static void PrintDeclarations()
        {
            foreach (var pair in container.Values())
            {
                Console.WriteLine(string.Format("{0} : {1}", pair.Key, Helpers.PrintType.Print(pair.Value.Type)));
            }
        }
    }
}
