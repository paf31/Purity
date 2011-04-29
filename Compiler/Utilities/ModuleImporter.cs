using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Modules;
using Purity.Compiler.Data;
using Purity.Core.Attributes;
using Purity.Compiler.Interfaces;
using Purity.Core;
using System.Reflection.Emit;
using Purity.Core.Types;

namespace Purity.Compiler.Utilities
{
    public static class ModuleImporter
    {
        public static System.Reflection.Module Import(string moduleName)
        {
            var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.dll");

            var modules = from file in files
                          let assembly = Assembly.LoadFile(file)
                          from m in assembly.GetModules()
                          where m.ScopeName.Equals(moduleName)
                          select m;

            System.Reflection.Module module;

            try
            {
                module = modules.Single();
            }
            catch (InvalidOperationException ex)
            {
                throw new ModuleImportException("Unable to locate unique module with name '" + moduleName + "'.", ex);
            }

            foreach (var type in module.Assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(ExportAttribute), false).Any())
                {
                    Container.Add(type.Name, new MockTypeDeclaration(type.GetGenericArguments().Select(p => p.Name).ToArray()));
                    TypeContainer.Add(type.Name, type);
                }
            }

            var dataClass = module.Assembly.GetTypes().FirstOrDefault(t => t.Name.Equals(Constants.DataClassName));

            foreach (var method in dataClass.GetMethods())
            {
                if (!method.GetParameters().Any())
                {
                    Type returnType = method.ReturnType;
                    
                    IType type = null;

                    try 
                    {
                        type = InferType(returnType); 
                    }
                    catch (CompilerException) 
                    {
                    }

                    if (type != null)
                    {
                        DataDeclaration declaration = new DataDeclaration(type, null);
                        declaration.TypeParameters = method.GetGenericArguments().Select(p => p.Name).ToArray();

                        DataContainer.Add(method.Name, method);
                        Container.Add(method.Name, declaration);
                    }
                }
            }

            return module;
        }

        private static IType InferType(Type type)
        {
            Type genericTypeDefinition = type.GetGenericArguments().Any() ? type.GetGenericTypeDefinition() : type;

            if (typeof(IFunction<,>).IsAssignableFrom(genericTypeDefinition))
            {
                Type left = type.GetGenericArguments()[0];
                Type right = type.GetGenericArguments()[1];
                return new Types.ArrowType(InferType(left), InferType(right));
            }
            else if (typeof(Either<,>).IsAssignableFrom(genericTypeDefinition))
            {
                Type left = type.GetGenericArguments()[0];
                Type right = type.GetGenericArguments()[1];
                return new Types.SumType(InferType(left), InferType(right));
            }
            else if (typeof(Tuple<,>).IsAssignableFrom(genericTypeDefinition))
            {
                Type left = type.GetGenericArguments()[0];
                Type right = type.GetGenericArguments()[1];
                return new Types.ProductType(InferType(left), InferType(right));
            }
            else if (type.IsGenericParameter)
            {
                return new Types.TypeParameter(type.Name);
            }
            else
            {
                Container.ResolveType(type.Name);
                return new Types.TypeSynonym(type.Name, type.GetGenericArguments().Select(InferType).ToArray());
            }
        }

        private class MockTypeDeclaration : ITypeDeclaration
        {
            public MockTypeDeclaration(string[] typeParameters)
            {
                TypeParameters = typeParameters;
            }

            public string[] TypeParameters
            {
                get;
                set;
            }

            public void AcceptVisitor(ITypeDeclarationVisitor visitor)
            {
                throw new NotSupportedException();
            }

            public R AcceptVisitor<R>(ITypeDeclarationVisitor<R> visitor)
            {
                throw new NotSupportedException();
            }
        }
    }
}
