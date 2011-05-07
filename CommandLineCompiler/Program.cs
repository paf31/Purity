using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Purity.Compiler;
using Purity.Compiler.Exceptions;
using System.Reflection;
using System.Reflection.Emit;
using Purity.Compiler.Parser;
using Purity.Compiler.Implementation;
using Purity.Compiler.Interfaces;

namespace CommandLineCompiler
{
    public class Program
    {
        static void Main(string[] args)
        {
            IMetadataContainer container = new MetadataContainer();
            IRuntimeContainer runtimeContainer = new RuntimeContainer();

            if (args.Length < 1)
            {
                Console.WriteLine("Usage: compile.exe <input-file> [<output-file>]");
            }
            else
            {
                var input = args[0];
                var output = args.Length >= 2 ? args[1] : "output.dll";

                StringBuilder program = new StringBuilder();

                using (var stream = new FileStream(input, FileMode.Open))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string nextLine;

                        while ((nextLine = reader.ReadLine()) != null)
                        {
                            if (!string.IsNullOrEmpty(nextLine) && nextLine[0] != Constants.StartOfComment)
                            {
                                program.AppendLine(nextLine);
                            }
                        }
                    }
                }

                try
                {
                    var parseResult = ModuleParser.ParseModule(program.ToString());

                    if (parseResult == null)
                    {
                        throw new CompilerException(ErrorMessages.UnableToParse);
                    }

                    var moduleDefinition = parseResult.Output;

                    var name = new AssemblyName(moduleDefinition.Name);
                    AppDomain domain = AppDomain.CurrentDomain;
                    var assembly = domain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save);
                    var module = assembly.DefineDynamicModule(moduleDefinition.Name, output);

                    var dataClass = module.DefineType(moduleDefinition.Name + '.' + Constants.DataClassName,
                        TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);

                    var compiler = new PurityCompiler(module, dataClass, container, runtimeContainer);

                    compiler.Compile(moduleDefinition);
                    compiler.CloseTypes();

                    assembly.Save(output);
                }
                catch (CompilerException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
