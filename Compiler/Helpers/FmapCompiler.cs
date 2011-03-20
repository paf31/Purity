using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Extensions;
using System.Reflection.Emit;
using Purity.Core;
using Purity.Core.Functions;

namespace Purity.Compiler.Helpers
{
    public class FmapCompiler : IFunctorVisitor
    {
        private readonly ILGenerator body;
        private readonly Type dom;
        private readonly Type cod;

        public FmapCompiler(ILGenerator body, Type dom, Type cod)
        {
            this.body = body;
            this.dom = dom;
            this.cod = cod;
        }

        public void VisitArrow(Functors.ArrowFunctor f)
        {
            f.Right.AcceptVisitor(this);

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(ArrowFunctorAction<,,>).MakeGenericType(
                    new TypeConverter(null).Convert(f.Left),
                    new FunctorTypeMapper(dom).Map(f.Right),
                    new FunctorTypeMapper(cod).Map(f.Right)),
                typeof(ArrowFunctorAction<,,>).GetConstructors()[0]));
        }

        public void VisitConstant(Functors.ConstantFunctor f)
        {
            var genericParameter = new TypeConverter(null).Convert(f.Value);
            body.Emit(OpCodes.Newobj, TypeBuilder.GetConstructor(
                typeof(IdentityFunction<>).MakeGenericType(genericParameter),
                typeof(IdentityFunction<>).GetConstructors()[0]));
        }

        public void VisitSynonym(Functors.FunctorSynonym f)
        {
            Container.ResolveFunctor(f.Identifier).AcceptVisitor(this);
        }

        public void VisitIdentity(Functors.IdentityFunctor f)
        {
            body.Emit(OpCodes.Ldarg_0);
        }

        public void VisitProduct(Functors.ProductFunctor f)
        {
            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(OutlFunction<,>).MakeGenericType(
                    new FunctorTypeMapper(dom).Map(f.Left),
                    new FunctorTypeMapper(dom).Map(f.Right)),
                typeof(OutlFunction<,>).GetConstructors()[0]));

            f.Left.AcceptVisitor(this);

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(CompositeFunction<,,>).MakeGenericType(
                    new FunctorTypeMapper(dom).Map(f),
                    new FunctorTypeMapper(dom).Map(f.Left),
                    new FunctorTypeMapper(cod).Map(f.Left)),
                typeof(CompositeFunction<,,>).GetConstructors()[0]));

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(OutrFunction<,>).MakeGenericType(
                    new FunctorTypeMapper(dom).Map(f.Left),
                    new FunctorTypeMapper(dom).Map(f.Right)),
                typeof(OutrFunction<,>).GetConstructors()[0]));

            f.Right.AcceptVisitor(this);

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(CompositeFunction<,,>).MakeGenericType(
                    new FunctorTypeMapper(dom).Map(f),
                    new FunctorTypeMapper(dom).Map(f.Right),
                    new FunctorTypeMapper(cod).Map(f.Right)),
                typeof(CompositeFunction<,,>).GetConstructors()[0]));

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(SplitFunction<,,>).MakeGenericType(
                    new FunctorTypeMapper(dom).Map(f),
                    new FunctorTypeMapper(cod).Map(f.Left),
                    new FunctorTypeMapper(cod).Map(f.Right)),
                typeof(SplitFunction<,,>).GetConstructors()[0]));
        }

        public void VisitSum(Functors.SumFunctor f)
        {
            f.Left.AcceptVisitor(this);

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(InlFunction<,>).MakeGenericType(
                    new FunctorTypeMapper(cod).Map(f.Left),
                    new FunctorTypeMapper(cod).Map(f.Right)),
                typeof(InlFunction<,>).GetConstructors()[0]));

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(CompositeFunction<,,>).MakeGenericType(
                    new FunctorTypeMapper(dom).Map(f.Left),
                    new FunctorTypeMapper(cod).Map(f.Left),
                    new FunctorTypeMapper(cod).Map(f)),
                typeof(CompositeFunction<,,>).GetConstructors()[0]));

            f.Right.AcceptVisitor(this);

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(InrFunction<,>).MakeGenericType(
                    new FunctorTypeMapper(cod).Map(f.Left),
                    new FunctorTypeMapper(cod).Map(f.Right)),
                typeof(InrFunction<,>).GetConstructors()[0]));

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(CompositeFunction<,,>).MakeGenericType(
                    new FunctorTypeMapper(dom).Map(f.Right),
                    new FunctorTypeMapper(cod).Map(f.Right),
                    new FunctorTypeMapper(cod).Map(f)),
                typeof(CompositeFunction<,,>).GetConstructors()[0]));

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(CaseFunction<,,>).MakeGenericType(
                    new FunctorTypeMapper(dom).Map(f.Left),
                    new FunctorTypeMapper(dom).Map(f.Right),
                    new FunctorTypeMapper(cod).Map(f)),
                typeof(CaseFunction<,,>).GetConstructors()[0]));
        }
    }
}
