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
using Purity.Compiler.TypeDeclarations;

namespace Purity.Compiler.Utilities
{
    public class ModuleImporter
    {
        private readonly IMetadataContainer container;
        private readonly IRuntimeContainer runtimeContainer;

        public ModuleImporter(IMetadataContainer container, IRuntimeContainer runtimeContainer)
        {
            this.container = container;
            this.runtimeContainer = runtimeContainer;
        }

        public System.Reflection.Module Import(string moduleName)
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
                var typeParameters = type.GetGenericArguments().Select(p => p.Name).ToArray();

                if (type.GetCustomAttributes(typeof(FiniteAttribute), false).Any())
                {
                    var cataMethod = type.GetMethod(Constants.CataMethodName);
                    var functor = ReflectionUtilities.InferFunctorFromFiniteType(cataMethod, "_T", container);

                    FiniteAttribute finiteAttribute = (FiniteAttribute) type.GetCustomAttributes(typeof(FiniteAttribute), false).Single();
                    container.Add(type.Name, new LFixTypeDeclaration(functor, "_T", typeParameters, 
                        finiteAttribute.ConstructorName, 
                        finiteAttribute.DestructorName, 
                        finiteAttribute.FoldName));
                    runtimeContainer.Add(type.Name, type);
                    runtimeContainer.AddDestructor(type.Name, finiteAttribute.DestructorClass);
                }

                if (type.GetCustomAttributes(typeof(InfiniteAttribute), false).Any())
                {
                    var applyMethod = type.GetMethod(Constants.ApplyMethodName);
                    var functor = ReflectionUtilities.InferFunctorFromInfiniteType(applyMethod, "_T", container);

                    InfiniteAttribute infiniteAttribute = (InfiniteAttribute) type.GetCustomAttributes(typeof(InfiniteAttribute), false).Single();
                    container.Add(type.Name, new GFixTypeDeclaration(functor, "_T", typeParameters,
                        infiniteAttribute.ConstructorName,
                        infiniteAttribute.DestructorName,
                        infiniteAttribute.UnfoldName));
                    runtimeContainer.Add(type.Name, type);
                    runtimeContainer.AddDestructor(type.Name, infiniteAttribute.DestructorClass);
                }

                if (type.GetCustomAttributes(typeof(SynonymAttribute), false).Any())
                {
                    var field = type.GetField(Constants.BoxedTypeValueFieldName);
                    var innerType = ReflectionUtilities.InferType(field.FieldType, container);

                    SynonymAttribute synonymAttribute = (SynonymAttribute) type.GetCustomAttributes(typeof(SynonymAttribute), false).Single();
                    container.Add(type.Name, new BoxedTypeDeclaration(innerType, typeParameters,
                        synonymAttribute.ConstructorName,
                        synonymAttribute.DestructorName));
                    runtimeContainer.Add(type.Name, type);
                    runtimeContainer.AddDestructor(type.Name, synonymAttribute.DestructorClass);
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
                        type = ReflectionUtilities.InferType(returnType, container); 
                    }
                    catch (CompilerException) 
                    {
                    }

                    if (type != null)
                    {
                        DataDeclaration declaration = new DataDeclaration(type, null);
                        declaration.TypeParameters = method.GetGenericArguments().Select(p => p.Name).ToArray();

                        container.Add(method.Name, declaration);
                        runtimeContainer.Add(method.Name, method);
                    }
                }
            }

            return module;
        }
    }
}
