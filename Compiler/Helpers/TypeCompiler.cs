﻿using System;
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
    public class TypeCompiler : ITypeDeclarationVisitor<Type>
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

        public static Type Compile(ITypeDeclaration typeDeclaration, TypeBuilder dataClass, ModuleBuilder module, string moduleName, string declarationName)
        {
            return typeDeclaration.AcceptVisitor(new TypeCompiler(module, dataClass, moduleName, declarationName));
        }

        public Type VisitBox(TypeDeclarations.BoxedTypeDeclaration t)
        {
            var compiler = new BoxedTypeCompiler(name, t, module, moduleName);

            TypeContainer.Add(name, compiler.Compile());

            var synonym = new Types.TypeSynonym(name, t.TypeParameters.Select(ident => new Types.TypeParameter(ident)).ToArray());

            var boxType = new Types.ArrowType(t.Type, synonym);
            var unboxType = new Types.ArrowType(synonym, t.Type);

            if (t.ConstructorFunctionName != null)
            {
                MethodCompiler.Compile(t.ConstructorFunctionName, dataClass, compiler.BoxFunctionConstructor,
                    boxType, t.TypeParameters);
            }

            if (t.DestructorFunctionName != null)
            {
                MethodCompiler.Compile(t.DestructorFunctionName, dataClass, compiler.UnboxFunctionConstructor,
                    unboxType, t.TypeParameters);
            }

            DataContainer.AddDestructor(name, compiler.UnboxFunction);

            return compiler.TypeBuilder;
        }

        public Type VisitLFix(TypeDeclarations.LFixTypeDeclaration t)
        {
            var functorClass = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name + Constants.MethodsSuffix,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed);

            var fmap = t.Functor.Compile(functorClass, t.TypeParameters);
            var compiler = new LFixCompiler(name, t, module, functorClass, fmap, moduleName);

            compiler.Compile();

            TypeContainer.Add(name, compiler.TypeBuilder);

            var synonym = new Types.TypeSynonym(name, t.TypeParameters.Select(ident => new Types.TypeParameter(ident)).ToArray());

            var fSynonym = FunctorApplication.Map(t.Functor, synonym);
            var constructorType = new Types.ArrowType(fSynonym, synonym);
            var destructorType = new Types.ArrowType(synonym, fSynonym);

            var cataParameter = new Types.TypeParameter(Constants.CataFunction1ClassGenericParameterName);
            var fCataParameter = FunctorApplication.Map(t.Functor, cataParameter);
            var cataType = new Types.ArrowType(new Types.ArrowType(fCataParameter, cataParameter), new Types.ArrowType(synonym, cataParameter));
            
            if (t.ConstructorFunctionName != null)
            {
                MethodCompiler.Compile(t.ConstructorFunctionName, dataClass, compiler.OutFunctionConstructor,
                    constructorType, t.TypeParameters);
            }

            if (t.DestructorFunctionName != null)
            {
                MethodCompiler.Compile(t.DestructorFunctionName, dataClass, compiler.InFunctionConstructor,
                    destructorType, t.TypeParameters);
            }

            if (t.CataFunctionName != null)
            {
                MethodCompiler.Compile(t.CataFunctionName, dataClass, compiler.CataFunction1Constructor,
                    cataType, new[] { Constants.CataFunction1ClassGenericParameterName }.Concat(t.TypeParameters).ToArray());
            }

            DataContainer.AddDestructor(name, compiler.InFunction);

            return compiler.TypeBuilder;
        }

        public Type VisitGFix(TypeDeclarations.GFixTypeDeclaration t)
        {
            var functorClass = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name + Constants.MethodsSuffix,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed);

            var fmap = t.Functor.Compile(functorClass, t.TypeParameters);
            var compiler = new GFixCompiler(name, t, module, functorClass, fmap, moduleName);

            compiler.Compile();

            TypeContainer.Add(name, compiler.TypeBuilder);

            var synonym = new Types.TypeSynonym(name, t.TypeParameters.Select(ident => new Types.TypeParameter(ident)).ToArray());

            var fSynonym = FunctorApplication.Map(t.Functor, synonym);
            var constructorType = new Types.ArrowType(fSynonym, synonym);
            var destructorType = new Types.ArrowType(synonym, fSynonym);

            var anaParameter = new Types.TypeParameter(Constants.AnaFunction1ClassGenericParameterName);
            var fAnaParameter = FunctorApplication.Map(t.Functor, anaParameter);
            var anaType = new Types.ArrowType(new Types.ArrowType(anaParameter, fAnaParameter), new Types.ArrowType(anaParameter, synonym));

            if (t.ConstructorFunctionName != null)
            {
                MethodCompiler.Compile(t.ConstructorFunctionName, dataClass, compiler.OutFunctionConstructor,
                   constructorType, t.TypeParameters);
            }

            if (t.DestructorFunctionName != null)
            {
                MethodCompiler.Compile(t.DestructorFunctionName, dataClass, compiler.InFunctionConstructor,
                    destructorType, t.TypeParameters);
            }

            if (t.AnaFunctionName != null)
            {
                MethodCompiler.Compile(t.AnaFunctionName, dataClass, compiler.AnaFunction1Constructor,
                    anaType, new[] { Constants.AnaFunction1ClassGenericParameterName }.Concat(t.TypeParameters).ToArray());
            }

            DataContainer.AddDestructor(name, compiler.InFunction);

            return compiler.TypeBuilder;
        }
    }
}
