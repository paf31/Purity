using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Extensions;
using System.Reflection.Emit;
using System.Reflection;
using Purity.Compiler.Modules;
using Purity.Compiler.Data;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Helpers
{
    public class TypeCompiler : ITypeDeclarationVisitor<ITypeInfo>
    {
        private readonly ModuleBuilder module;
        private readonly TypeBuilder dataClass;
        private readonly string moduleName;
        private readonly string name;

        public TypeCompiler(ModuleBuilder module, TypeBuilder dataClass, string moduleName, string name)
        {
            this.module = module;
            this.dataClass = dataClass;
            this.moduleName = moduleName;
            this.name = name;
        }

        public static ITypeInfo Compile(ITypeDeclaration typeDeclaration, TypeBuilder dataClass, ModuleBuilder module, string moduleName, string declarationName)
        {
            return typeDeclaration.AcceptVisitor(new TypeCompiler(module, dataClass, moduleName, declarationName));
        }

        public ITypeInfo VisitBox(TypeDeclarations.BoxedTypeDeclaration t)
        {
            var typeInfo = BoxedTypeCreator.CreateBoxedType(t.Type, module, moduleName, name, t.TypeParameters);

            TypeContainer.Add(name, typeInfo);

            var synonym = new Types.TypeSynonym(name, t.TypeParameters.Select(ident => new Types.TypeParameter(ident)).ToArray());

            var boxType = new Types.ArrowType(t.Type, synonym);
            var unboxType = new Types.ArrowType(synonym, t.Type);

            if (t.ConstructorFunctionName != null)
            {
                MethodCompiler.Compile(t.ConstructorFunctionName, dataClass, typeInfo.BoxFunctionConstructor,
                    boxType, t.TypeParameters);
            }

            if (t.DestructorFunctionName != null)
            {
                MethodCompiler.Compile(t.DestructorFunctionName, dataClass, typeInfo.UnboxFunctionConstructor,
                    unboxType, t.TypeParameters);
            }
            return typeInfo;
        }

        public ITypeInfo VisitLFix(TypeDeclarations.LFixTypeDeclaration t)
        {
            var functorClass = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name + Constants.MethodsSuffix,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed);

            var fmap = t.Functor.Compile(functorClass, t.TypeParameters);
            var named = new Named<IFunctor>(name, t.Functor);
            var compiler = new LFixCompiler(named, module, functorClass, fmap, moduleName, t.TypeParameters);

            compiler.Compile();

            var typeInfo = compiler.TypeInfo;

            TypeContainer.Add(name, typeInfo);

            var synonym = new Types.TypeSynonym(name, t.TypeParameters.Select(ident => new Types.TypeParameter(ident)).ToArray());

            var fSynonym = FunctorApplication.Map(t.Functor, synonym);
            var constructorType = new Types.ArrowType(fSynonym, synonym);
            var destructorType = new Types.ArrowType(synonym, fSynonym);

            var cataParameter = new Types.TypeParameter(Constants.CataFunction1ClassGenericParameterName);
            var fCataParameter = FunctorApplication.Map(t.Functor, cataParameter);
            var cataType = new Types.ArrowType(new Types.ArrowType(fCataParameter, cataParameter), new Types.ArrowType(synonym, cataParameter));
            
            if (t.ConstructorFunctionName != null)
            {
                MethodCompiler.Compile(t.ConstructorFunctionName, dataClass, typeInfo.OutFunctionConstructor,
                    constructorType, t.TypeParameters);
            }

            if (t.DestructorFunctionName != null)
            {
                MethodCompiler.Compile(t.DestructorFunctionName, dataClass, typeInfo.InFunctionConstructor,
                    destructorType, t.TypeParameters);
            }

            if (t.CataFunctionName != null)
            {
                MethodCompiler.Compile(t.CataFunctionName, dataClass, typeInfo.CataFunction1Constructor,
                    cataType, new[] { Constants.CataFunction1ClassGenericParameterName }.Concat(t.TypeParameters).ToArray());
            }

            return typeInfo;
        }

        public ITypeInfo VisitGFix(TypeDeclarations.GFixTypeDeclaration t)
        {
            var functorClass = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name + Constants.MethodsSuffix,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed);

            var fmap = t.Functor.Compile(functorClass, t.TypeParameters);
            var named = new Named<IFunctor>(name, t.Functor);
            var compiler = new GFixCompiler(named, module, functorClass, fmap, moduleName, t.TypeParameters);

            compiler.Compile();

            var typeInfo = compiler.TypeInfo;

            TypeContainer.Add(name, typeInfo);

            var synonym = new Types.TypeSynonym(name, t.TypeParameters.Select(ident => new Types.TypeParameter(ident)).ToArray());

            var fSynonym = FunctorApplication.Map(t.Functor, synonym);
            var constructorType = new Types.ArrowType(fSynonym, synonym);
            var destructorType = new Types.ArrowType(synonym, fSynonym);

            var anaParameter = new Types.TypeParameter(Constants.AnaFunction1ClassGenericParameterName);
            var fAnaParameter = FunctorApplication.Map(t.Functor, anaParameter);
            var anaType = new Types.ArrowType(new Types.ArrowType(anaParameter, fAnaParameter), new Types.ArrowType(anaParameter, synonym));

            if (t.ConstructorFunctionName != null)
            {
                MethodCompiler.Compile(t.ConstructorFunctionName, dataClass, typeInfo.OutFunctionConstructor,
                   constructorType, t.TypeParameters);
            }

            if (t.DestructorFunctionName != null)
            {
                MethodCompiler.Compile(t.DestructorFunctionName, dataClass, typeInfo.InFunctionConstructor,
                    destructorType, t.TypeParameters);
            }

            if (t.AnaFunctionName != null)
            {
                MethodCompiler.Compile(t.AnaFunctionName, dataClass, typeInfo.AnaFunction1Constructor,
                    anaType, new[] { Constants.AnaFunction1ClassGenericParameterName }.Concat(t.TypeParameters).ToArray());
            }

            return typeInfo;
        }
    }
}
