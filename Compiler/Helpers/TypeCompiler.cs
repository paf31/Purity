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

namespace Purity.Compiler.Helpers
{
    public class TypeCompiler : ITypeVisitor
    {
        private readonly ModuleBuilder module;
        private readonly string moduleName;
        private readonly string name;

        public ITypeInfo Result
        {
            get;
            set;
        }

        public TypeCompiler(ModuleBuilder module, string moduleName, string name)
        {
            this.module = module;
            this.moduleName = moduleName;
            this.name = name;
        }

        public void VisitArrow(Types.ArrowType t)
        {
            Result = BoxedTypeCreator.CreateBoxedType(t, module, moduleName, name);
        }

        public void VisitSynonym(Types.TypeSynonym t)
        {
            Result = BoxedTypeCreator.CreateBoxedType(t, module, moduleName, name);
        }

        public void VisitProduct(Types.ProductType t)
        {
            Result = BoxedTypeCreator.CreateBoxedType(t, module, moduleName, name);
        }

        public void VisitSum(Types.SumType t)
        {
            Result = BoxedTypeCreator.CreateBoxedType(t, module, moduleName, name);
        }

        public void VisitLFix(Types.LFixType t)
        {
            t.Identifier = name;

            var functorClass = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name + Constants.MethodsSuffix,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed);

            var fmap = t.Functor.Compile(functorClass);
            var named = new Named<IFunctor>(name, t.Functor);
            var compiler = new LFixCompiler(named, module, functorClass, fmap, moduleName);
            compiler.Compile();

            Result = compiler.TypeInfo;
        }

        public void VisitGFix(Types.GFixType t)
        {
            t.Identifier = name;

            var functorClass = module.DefineType(moduleName + '.' + Constants.TypesNamespace + '.' + name + Constants.MethodsSuffix,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed);

            var fmap = t.Functor.Compile(functorClass);
            var named = new Named<IFunctor>(name, t.Functor);
            var compiler = new GFixCompiler(named, module, functorClass, fmap, moduleName);
            compiler.Compile();

            Result = compiler.TypeInfo;
        }
    }
}
