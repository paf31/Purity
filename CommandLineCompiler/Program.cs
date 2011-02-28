using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Purity.Compiler;
using Purity.Compiler.Exceptions;

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

                string program;

                using (var stream = new FileStream(input, FileMode.Open))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        program = reader.ReadToEnd();
                    }
                }

                try
                {
                    new PurityCompiler(program).Compile(output, moduleName);
                }
                catch (CompilerException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
