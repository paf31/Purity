using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Extensions;
using System.Reflection.Emit;
using Purity.Core;
using Purity.Compiler.Types;

namespace Purity.Compiler.Helpers
{
    public class DataCompiler : ITypedExpressionVisitor
    {
        private readonly ILGenerator body;

        public DataCompiler(ILGenerator body)
        {
            this.body = body;
        }

        public void VisitAna(TypedExpressions.Ana d)
        {
            d.Coalgebra.AcceptVisitor(this);
            var typeInfo = TypeContainer.ResolveGFixType(d.Functor);
            var anaClass = typeInfo.AnaFunction;
            var genericCtor = typeInfo.AnaFunctionConstructor;
            var genericParameter = new TypeConverter().Convert(d.CarrierType);
            var ctor = TypeBuilder.GetConstructor(anaClass.MakeGenericType(genericParameter), genericCtor);
            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitCata(TypedExpressions.Cata d)
        {
            d.Algebra.AcceptVisitor(this);
            var typeInfo = TypeContainer.ResolveLFixType(d.Functor);
            var cataClass = typeInfo.CataFunction;
            var genericCtor = typeInfo.CataFunctionConstructor;
            var genericParameter = new TypeConverter().Convert(d.CarrierType);
            var ctor = TypeBuilder.GetConstructor(cataClass.MakeGenericType(genericParameter), genericCtor);
            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitIn(TypedExpressions.In d)
        {
            if (d.Source is LFixType)
            {
                var ctor = TypeContainer.ResolveLFixType(d.Functor).InFunctionConstructor;
                body.Emit(OpCodes.Newobj, ctor);
            }
            else if (d.Source is GFixType)
            {
                var ctor = TypeContainer.ResolveGFixType(d.Functor).InFunctionConstructor;
                body.Emit(OpCodes.Newobj, ctor);
            }
        }

        public void VisitOut(TypedExpressions.Out d)
        {
            if (d.Target is LFixType)
            {
                var ctor = TypeContainer.ResolveLFixType(d.Functor).OutFunctionConstructor;
                body.Emit(OpCodes.Newobj, ctor);
            }
            else if (d.Target is GFixType)
            {
                var ctor = TypeContainer.ResolveGFixType(d.Functor).OutFunctionConstructor;
                body.Emit(OpCodes.Newobj, ctor);
            }
        }

        public void VisitApplication(TypedExpressions.Application d)
        {
            d.Left.AcceptVisitor(this);
            d.Right.AcceptVisitor(this);

            var dom = new TypeConverter().Convert(d.LeftType);
            var functionType = new TypeConverter().Convert(d.LeftType);
            var callMethod = typeof(IFunction<,>).GetMethod("Call");

            body.Emit(OpCodes.Callvirt, TypeBuilder.GetMethod(functionType, callMethod));
        }

        public void VisitCase(TypedExpressions.Case d)
        {
            d.Left.AcceptVisitor(this);
            d.Right.AcceptVisitor(this);

            var leftType = new TypeConverter().Convert(d.LeftType);
            var rightType = new TypeConverter().Convert(d.RightType);
            var resultType = new TypeConverter().Convert(d.ResultType);

            var defCtor = typeof(CaseFunction<,,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(CaseFunction<,,>).MakeGenericType(leftType, rightType, resultType), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitComposition(TypedExpressions.Composition d)
        {
            d.Right.AcceptVisitor(this);
            d.Left.AcceptVisitor(this);

            var leftType = new TypeConverter().Convert(d.LeftType);
            var middleType = new TypeConverter().Convert(d.MiddleType);
            var rightType = new TypeConverter().Convert(d.RightType);

            var defCtor = typeof(CompositeFunction<,,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(CompositeFunction<,,>).MakeGenericType(leftType, middleType, rightType), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitConst(TypedExpressions.Const d)
        {
            d.Value.AcceptVisitor(this);

            var dom = new TypeConverter().Convert(d.InputType);
            var cod = new TypeConverter().Convert(d.OutputType);

            var defCtor = typeof(ConstantFunction<,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(ConstantFunction<,>).MakeGenericType(dom, cod), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitIdentity(TypedExpressions.Identity d)
        {
            var type = new TypeConverter().Convert(d.Type);

            var defCtor = typeof(IdentityFunction<>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(IdentityFunction<>).MakeGenericType(type), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitInl(TypedExpressions.Inl d)
        {
            var leftType = new TypeConverter().Convert(d.LeftType);
            var rightType = new TypeConverter().Convert(d.RightType);

            var defCtor = typeof(InlFunction<,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(InlFunction<,>).MakeGenericType(leftType, rightType), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitInr(TypedExpressions.Inr d)
        {
            var leftType = new TypeConverter().Convert(d.LeftType);
            var rightType = new TypeConverter().Convert(d.RightType);

            var defCtor = typeof(InrFunction<,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(InrFunction<,>).MakeGenericType(leftType, rightType), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitOutl(TypedExpressions.Outl d)
        {
            var leftType = new TypeConverter().Convert(d.LeftType);
            var rightType = new TypeConverter().Convert(d.RightType);

            var defCtor = typeof(OutlFunction<,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(OutlFunction<,>).MakeGenericType(leftType, rightType), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitOutr(TypedExpressions.Outr d)
        {
            var leftType = new TypeConverter().Convert(d.LeftType);
            var rightType = new TypeConverter().Convert(d.RightType);

            var defCtor = typeof(OutrFunction<,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(OutrFunction<,>).MakeGenericType(leftType, rightType), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitSplit(TypedExpressions.Split d)
        {
            d.Left.AcceptVisitor(this);
            d.Right.AcceptVisitor(this);

            var inputType = new TypeConverter().Convert(d.InputType);
            var leftType = new TypeConverter().Convert(d.LeftType);
            var rightType = new TypeConverter().Convert(d.RightType);

            var defCtor = typeof(SplitFunction<,,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(SplitFunction<,,>).MakeGenericType(inputType, leftType, rightType), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitUncurry(TypedExpressions.Uncurried d)
        {
            d.Function.AcceptVisitor(this);

            var first = new TypeConverter().Convert(d.First);
            var second = new TypeConverter().Convert(d.Second);
            var output = new TypeConverter().Convert(d.Output);

            var defCtor = typeof(UncurriedFunction<,,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(UncurriedFunction<,,>).MakeGenericType(first, second, output), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitCurry(TypedExpressions.Curried d)
        {
            d.Function.AcceptVisitor(this);

            var first = new TypeConverter().Convert(d.First);
            var second = new TypeConverter().Convert(d.Second);
            var output = new TypeConverter().Convert(d.Output);

            var defCtor = typeof(CurriedFunction<,,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(CurriedFunction<,,>).MakeGenericType(first, second, output), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitCl(TypedExpressions.Cl d)
        {
            d.Function.AcceptVisitor(this);

            var a = new TypeConverter().Convert(d.A);
            var b = new TypeConverter().Convert(d.B);
            var c = new TypeConverter().Convert(d.C);

            var defCtor = typeof(Cl<,,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(Cl<,,>).MakeGenericType(a, b, c), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitCr(TypedExpressions.Cr d)
        {
            d.Function.AcceptVisitor(this);

            var a = new TypeConverter().Convert(d.A);
            var b = new TypeConverter().Convert(d.B);
            var c = new TypeConverter().Convert(d.C);

            var defCtor = typeof(Cr<,,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(Cr<,,>).MakeGenericType(a, b, c), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }
    }
}
