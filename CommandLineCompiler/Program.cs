using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Purity.Compiler;
using Purity.Compiler.Exceptions;
using System.Reflection;
using System.Reflection.Emit;

namespace CommandLineCompiler
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: compile.exe <input-file> [<output-file>]");
            }
            else
            {
                var input = args[0];
                var output = args.Length >= 2 ? args[1] : "output.dll";

                string moduleName;

                if (input.Contains('.'))
                {
                    moduleName = input.Substring(0, input.IndexOf('.'));
                }
                else
                {
                    moduleName = input;
                }

                if (string.IsNullOrEmpty(moduleName))
                {
                    moduleName = "output";
                }

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
                    var name = new AssemblyName(moduleName);
                    AppDomain domain = AppDomain.CurrentDomain;
                    var assembly = domain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save);
                    var module = assembly.DefineDynamicModule(moduleName, output);

                    var dataClass = module.DefineType(moduleName + '.' + Constants.DataClassName,
                        TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract);

                    var compiler = new PurityCompiler(moduleName, module, dataClass);

                    compiler.Compile(program.ToString());
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
