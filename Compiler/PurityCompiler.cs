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
                throw new CompilerException("Unable to parse input.");
            }

            Container.AddAll(parseResult.Output);

            var declarations =
                parseResult.Output.Elements
                .Where(element => element.ElementType == ProgramElementType.Data)
                .Select(element => 
                {
                    try
                    {
                        return ExtensionMethods.RemoveSynonyms(element.Data);
                    }
                    catch (CompilerException ex) 
                    {
                        throw new CompilerException(string.Format("Error in declaration '{0}': {1}", 
                            element.Data.Name, ex.Message), ex);
                    }
                })
                .ToArray();

            var name = new AssemblyName(moduleName);
            AppDomain domain = AppDomain.CurrentDomain;
            var assembly = domain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save);
            var module = assembly.DefineDynamicModule(moduleName, filename);
            
            var moduleClass = module.DefineType(moduleName + "." + moduleName,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed);

            var body = moduleClass.DefineTypeInitializer().GetILGenerator();

            foreach (var decl in declarations)
            {
                try
                {
                    var typeChecker = new Checker(decl.Value);
                    var typed = typeChecker.CreateTypedExpression();
                    var tableau = typeChecker.Tableau;

                    foreach (var lfix in tableau.Types.Values
                        .OfType<Typechecker.Types.LFixType>()
                        .Select(t => new FunctorConverter().Convert(t.Functor))
                        .Where(f => !TypeContainer.HasLFixType(f)))
                    {
                        string functorName = lfix.ToString();

                        var functorClass = module.DefineType(moduleName + "." + functorName + "Methods",
                            TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed);

                        var fmap = lfix.Compile(functorClass);
                        var named = new Named<IFunctor>(functorName, lfix);
                        var compiler = new LFixCompiler(named, module, functorClass, fmap, moduleName);
                        compiler.Compile();

                        TypeContainer.Add(lfix, compiler.TypeInfo);
                    }

                    foreach (var gfix in tableau.Types.Values
                        .OfType<Typechecker.Types.GFixType>()
                        .Select(t => new FunctorConverter().Convert(t.Functor))
                        .Where(f => !TypeContainer.HasGFixType(f)))
                    {
                        string functorName = gfix.ToString();

                        var functorClass = module.DefineType(moduleName + ".Co" + functorName + "Methods",
                            TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed);

                        var fmap = gfix.Compile(functorClass);
                        var named = new Named<IFunctor>(functorName, gfix);
                        var compiler = new GFixCompiler(named, module, functorClass, fmap, moduleName);
                        compiler.Compile();

                        TypeContainer.Add(gfix, compiler.TypeInfo);
                    }

                    var type = new Purity.Compiler.Helpers.TypeConverter().Convert(decl.Value.Type);
                    var field = moduleClass.DefineField(decl.Name, type, FieldAttributes.Public | FieldAttributes.Static);
                    typed.AcceptVisitor(new DataCompiler(body));
                    body.Emit(OpCodes.Stsfld, field);
                }
                catch (CompilerException ex) 
                {
                    throw new CompilerException(string.Format("Error in declaration '{0}': {1}", decl.Name, ex.Message), ex);
                }
            }

            body.Emit(OpCodes.Ret);

            module.GetTypes().OfType<TypeBuilder>().ToList().ForEach(t => t.CreateType());
      
            assembly.Save(filename);
        }
    }
}
