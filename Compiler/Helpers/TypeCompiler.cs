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
    public class TypeCompiler : ITypeVisitor<ITypeInfo>
    {
        private readonly ModuleBuilder module;
        private readonly string moduleName;
        private readonly string name;

        public TypeCompiler(ModuleBuilder module, string moduleName, string name)
        {
            this.module = module;
            this.moduleName = moduleName;
            this.name = name;
        }

        public static ITypeInfo Compile(IType type, ModuleBuilder module, string moduleName, string declarationName)
        {
            return type.AcceptVisitor(new TypeCompiler(module, moduleName, declarationName));
        }

        public ITypeInfo VisitArrow(Types.ArrowType t)
        {
            return BoxedTypeCreator.CreateBoxedType(t, module, moduleName, name);
        }

        public ITypeInfo VisitSynonym(Types.TypeSynonym t)
        {
            return BoxedTypeCreator.CreateBoxedType(t, module, moduleName, name);
        }

        public ITypeInfo VisitProduct(Types.ProductType t)
        {
            return BoxedTypeCreator.CreateBoxedType(t, module, moduleName, name);
        }

        public ITypeInfo VisitSum(Types.SumType t)
        {
            return BoxedTypeCreator.CreateBoxedType(t, module, moduleName, name);
        }

        public ITypeInfo VisitLFix(Types.LFixType t)
        {
            t.Identifier = name;

            var functorClass = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name + Constants.MethodsSuffix,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed);

            var fmap = t.Functor.Compile(functorClass);
            var named = new Named<IFunctor>(name, t.Functor);
            var compiler = new LFixCompiler(named, module, functorClass, fmap, moduleName);
            compiler.Compile();

            return compiler.TypeInfo;
        }

        public ITypeInfo VisitGFix(Types.GFixType t)
        {
            t.Identifier = name;

            var functorClass = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name + Constants.MethodsSuffix,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed);

            var fmap = t.Functor.Compile(functorClass);
            var named = new Named<IFunctor>(name, t.Functor);
            var compiler = new GFixCompiler(named, module, functorClass, fmap, moduleName);
            compiler.Compile();

            return compiler.TypeInfo;
        }

        public ITypeInfo VisitParameter(Types.TypeParameter t)
        {
            throw new CompilerException(ErrorMessages.UnexpectedVariable);
        }
    }
}
