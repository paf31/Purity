using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using System.Reflection.Emit;
using System.Reflection;
using Purity.Compiler.Modules;
using Purity.Compiler.Data;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Utilities;

namespace Purity.Compiler.Helpers
{
    public class TypeCompiler : ITypeDeclarationVisitor<Type>
    {
        private readonly ModuleBuilder module;
        private readonly TypeBuilder dataClass;
        private readonly string moduleName;
        private readonly string name;
        private readonly IMetadataContainer container;
        private readonly IRuntimeContainer runtimeContainer;

        public TypeCompiler(ModuleBuilder module, TypeBuilder dataClass, string moduleName, string name,
            IMetadataContainer container, IRuntimeContainer runtimeContainer)
        {
            this.module = module;
            this.dataClass = dataClass;
            this.moduleName = moduleName;
            this.name = name;
            this.container = container;
            this.runtimeContainer = runtimeContainer;
        }

        public static Type Compile(ITypeDeclaration typeDeclaration, TypeBuilder dataClass, ModuleBuilder module,
            string moduleName, string declarationName, IMetadataContainer container, IRuntimeContainer runtimeContainer)
        {
            return typeDeclaration.AcceptVisitor(new TypeCompiler(module, dataClass, moduleName,
                declarationName, container, runtimeContainer));
        }

        public Type VisitBox(TypeDeclarations.BoxedTypeDeclaration t)
        {
            var compiler = new BoxedTypeCompiler(name, t, module, moduleName, runtimeContainer);

            runtimeContainer.Add(name, compiler.Compile());

            var synonym = new Types.TypeSynonym(name, t.TypeParameters.Select(ident => new Types.TypeParameter(ident)).ToArray());

            var boxType = new Types.ArrowType(t.Type, synonym);
            var unboxType = new Types.ArrowType(synonym, t.Type);

            if (t.ConstructorFunctionName != null)
            {
                MethodCompiler.Compile(t.ConstructorFunctionName, dataClass, compiler.BoxFunctionConstructor,
                    boxType, t.TypeParameters, container, runtimeContainer);
            }

            if (t.DestructorFunctionName != null)
            {
                MethodCompiler.Compile(t.DestructorFunctionName, dataClass, compiler.UnboxFunctionConstructor,
                    unboxType, t.TypeParameters, container, runtimeContainer);
            }

            runtimeContainer.AddDestructor(name, compiler.UnboxFunction);

            return compiler.TypeBuilder;
        }

        public Type VisitLFix(TypeDeclarations.LFixTypeDeclaration t)
        {
            var functorClass = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name + Constants.MethodsSuffix,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed);

            var fmap = FunctorMethods.Compile(t.Type, t.VariableName, functorClass, t.TypeParameters, runtimeContainer);
            var compiler = new LFixCompiler(name, t, module, functorClass, fmap, moduleName, runtimeContainer);

            compiler.Compile();

            runtimeContainer.Add(name, compiler.TypeBuilder);

            var synonym = new Types.TypeSynonym(name, t.TypeParameters.Select(ident => new Types.TypeParameter(ident)).ToArray());

            var fSynonym = FunctorApplication.Map(t.VariableName, t.Type, synonym);
            var constructorType = new Types.ArrowType(fSynonym, synonym);
            var destructorType = new Types.ArrowType(synonym, fSynonym);

            var cataParameter = new Types.TypeParameter(Constants.CataFunction1ClassGenericParameterName);
            var fCataParameter = FunctorApplication.Map(t.VariableName, t.Type, cataParameter);
            var cataType = new Types.ArrowType(new Types.ArrowType(fCataParameter, cataParameter), new Types.ArrowType(synonym, cataParameter));
            
            if (t.ConstructorFunctionName != null)
            {
                MethodCompiler.Compile(t.ConstructorFunctionName, dataClass, compiler.OutFunctionConstructor,
                    constructorType, t.TypeParameters, container, runtimeContainer);
            }

            if (t.DestructorFunctionName != null)
            {
                MethodCompiler.Compile(t.DestructorFunctionName, dataClass, compiler.InFunctionConstructor,
                    destructorType, t.TypeParameters, container, runtimeContainer);
            }

            if (t.CataFunctionName != null)
            {
                MethodCompiler.Compile(t.CataFunctionName, dataClass, compiler.CataFunction1Constructor,
                    cataType, new[] { Constants.CataFunction1ClassGenericParameterName }.Concat(t.TypeParameters).ToArray(), 
                    container, runtimeContainer);
            }

            runtimeContainer.AddDestructor(name, compiler.InFunction);

            return compiler.TypeBuilder;
        }

        public Type VisitGFix(TypeDeclarations.GFixTypeDeclaration t)
        {
            var functorClass = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name + Constants.MethodsSuffix,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed);

            var fmap = FunctorMethods.Compile(t.Type, t.VariableName, functorClass, t.TypeParameters, runtimeContainer);
            var compiler = new GFixCompiler(name, t, module, functorClass, fmap, moduleName, runtimeContainer);

            compiler.Compile();

            runtimeContainer.Add(name, compiler.TypeBuilder);

            var synonym = new Types.TypeSynonym(name, t.TypeParameters.Select(ident => new Types.TypeParameter(ident)).ToArray());

            var fSynonym = FunctorApplication.Map(t.VariableName, t.Type, synonym);
            var constructorType = new Types.ArrowType(fSynonym, synonym);
            var destructorType = new Types.ArrowType(synonym, fSynonym);

            var anaParameter = new Types.TypeParameter(Constants.AnaFunction1ClassGenericParameterName);
            var fAnaParameter = FunctorApplication.Map(t.VariableName, t.Type, anaParameter);
            var anaType = new Types.ArrowType(new Types.ArrowType(anaParameter, fAnaParameter), new Types.ArrowType(anaParameter, synonym));

            if (t.ConstructorFunctionName != null)
            {
                MethodCompiler.Compile(t.ConstructorFunctionName, dataClass, compiler.OutFunctionConstructor,
                   constructorType, t.TypeParameters, container, runtimeContainer);
            }

            if (t.DestructorFunctionName != null)
            {
                MethodCompiler.Compile(t.DestructorFunctionName, dataClass, compiler.InFunctionConstructor,
                    destructorType, t.TypeParameters, container, runtimeContainer);
            }

            if (t.AnaFunctionName != null)
            {
                MethodCompiler.Compile(t.AnaFunctionName, dataClass, compiler.AnaFunction1Constructor,
                    anaType, new[] { Constants.AnaFunction1ClassGenericParameterName }.Concat(t.TypeParameters).ToArray(),
                    container, runtimeContainer);
            }

            runtimeContainer.AddDestructor(name, compiler.InFunction);

            return compiler.TypeBuilder;
        }
    }
}
