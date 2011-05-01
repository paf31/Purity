using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Extensions;
using System.Reflection.Emit;
using Purity.Compiler.Types;
using Purity.Compiler.Functors;
using System.Reflection;
using Purity.Core;
using Purity.Core.Types;
using Purity.Core.Functions;
using Purity.Compiler.Data;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Modules;

namespace Purity.Compiler.Helpers
{
    public class DataCompiler : ITypedExpressionVisitor
    {
        private readonly ILGenerator body;
        private Type[] genericMethodParameters;

        public DataCompiler(ILGenerator body, Type[] genericMethodParameters)
        {
            this.body = body;
            this.genericMethodParameters = genericMethodParameters;
        }

        public void VisitApplication(TypedExpressions.Application d)
        {
            d.Left.AcceptVisitor(this);
            d.Right.AcceptVisitor(this);

            var functionType = new TypeConverter(genericMethodParameters).Convert(d.LeftType);
            var callMethod = typeof(IFunction<,>).GetMethod(Constants.CallMethodName);

            body.Emit(OpCodes.Callvirt, TypeBuilder.GetMethod(functionType, callMethod));
        }

        public void VisitCase(TypedExpressions.Case d)
        {
            d.Left.AcceptVisitor(this);
            d.Right.AcceptVisitor(this);

            var leftType = new TypeConverter(genericMethodParameters).Convert(d.LeftType);
            var rightType = new TypeConverter(genericMethodParameters).Convert(d.RightType);
            var resultType = new TypeConverter(genericMethodParameters).Convert(d.ResultType);

            var defCtor = typeof(CaseFunction<,,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(CaseFunction<,,>).MakeGenericType(leftType, rightType, resultType), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitComposition(TypedExpressions.Composition d)
        {
            d.Right.AcceptVisitor(this);
            d.Left.AcceptVisitor(this);

            var leftType = new TypeConverter(genericMethodParameters).Convert(d.LeftType);
            var middleType = new TypeConverter(genericMethodParameters).Convert(d.MiddleType);
            var rightType = new TypeConverter(genericMethodParameters).Convert(d.RightType);

            var defCtor = typeof(CompositeFunction<,,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(CompositeFunction<,,>).MakeGenericType(leftType, middleType, rightType), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitConst(TypedExpressions.Const d)
        {
            d.Value.AcceptVisitor(this);

            var dom = new TypeConverter(genericMethodParameters).Convert(d.InputType);
            var cod = new TypeConverter(genericMethodParameters).Convert(d.OutputType);

            var defCtor = typeof(ConstantFunction<,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(ConstantFunction<,>).MakeGenericType(dom, cod), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitIdentity(TypedExpressions.Identity d)
        {
            var type = new TypeConverter(genericMethodParameters).Convert(d.Type);

            var defCtor = typeof(IdentityFunction<>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(IdentityFunction<>).MakeGenericType(type), defCtor);
            
            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitInl(TypedExpressions.Inl d)
        {
            var leftType = new TypeConverter(genericMethodParameters).Convert(d.LeftType);
            var rightType = new TypeConverter(genericMethodParameters).Convert(d.RightType);

            var defCtor = typeof(InlFunction<,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(InlFunction<,>).MakeGenericType(leftType, rightType), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitInr(TypedExpressions.Inr d)
        {
            var leftType = new TypeConverter(genericMethodParameters).Convert(d.LeftType);
            var rightType = new TypeConverter(genericMethodParameters).Convert(d.RightType);

            var defCtor = typeof(InrFunction<,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(InrFunction<,>).MakeGenericType(leftType, rightType), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitOutl(TypedExpressions.Outl d)
        {
            var leftType = new TypeConverter(genericMethodParameters).Convert(d.LeftType);
            var rightType = new TypeConverter(genericMethodParameters).Convert(d.RightType);

            var defCtor = typeof(OutlFunction<,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(OutlFunction<,>).MakeGenericType(leftType, rightType), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitOutr(TypedExpressions.Outr d)
        {
            var leftType = new TypeConverter(genericMethodParameters).Convert(d.LeftType);
            var rightType = new TypeConverter(genericMethodParameters).Convert(d.RightType);

            var defCtor = typeof(OutrFunction<,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(OutrFunction<,>).MakeGenericType(leftType, rightType), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitSplit(TypedExpressions.Split d)
        {
            d.Left.AcceptVisitor(this);
            d.Right.AcceptVisitor(this);

            var inputType = new TypeConverter(genericMethodParameters).Convert(d.InputType);
            var leftType = new TypeConverter(genericMethodParameters).Convert(d.LeftType);
            var rightType = new TypeConverter(genericMethodParameters).Convert(d.RightType);

            var defCtor = typeof(SplitFunction<,,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(SplitFunction<,,>).MakeGenericType(inputType, leftType, rightType), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitUncurry(TypedExpressions.Uncurried d)
        {
            d.Function.AcceptVisitor(this);

            var first = new TypeConverter(genericMethodParameters).Convert(d.First);
            var second = new TypeConverter(genericMethodParameters).Convert(d.Second);
            var output = new TypeConverter(genericMethodParameters).Convert(d.Output);

            var defCtor = typeof(UncurriedFunction<,,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(UncurriedFunction<,,>).MakeGenericType(first, second, output), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitCurry(TypedExpressions.Curried d)
        {
            d.Function.AcceptVisitor(this);

            var first = new TypeConverter(genericMethodParameters).Convert(d.First);
            var second = new TypeConverter(genericMethodParameters).Convert(d.Second);
            var output = new TypeConverter(genericMethodParameters).Convert(d.Output);

            var defCtor = typeof(CurriedFunction<,,>).GetConstructors()[0];
            var ctor = TypeBuilder.GetConstructor(typeof(CurriedFunction<,,>).MakeGenericType(first, second, output), defCtor);

            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitSynonym(TypedExpressions.DataSynonym d)
        {
            var method = DataContainer.Resolve(d.Identifier);

            if (d.TypeParameters.Any())
            {
                var typeParameters = new Type[d.TypeParameters.Count];

                for (int i = 0; i < typeParameters.Length; i++)
                {
                    typeParameters[i] = new TypeConverter(genericMethodParameters).Convert(d.TypeParameters.Values.ElementAt(i));
                }

                var genericMethod = method.MakeGenericMethod(typeParameters);

                body.Emit(OpCodes.Call, genericMethod);
            }
            else
            {
                body.Emit(OpCodes.Call, method);
            }
        }

        public void VisitAbstraction(TypedExpressions.Abstraction d)
        {
            d.PointFreeExpression.AcceptVisitor(this);
        }

        public void VisitVariable(TypedExpressions.Variable d)
        {
            throw new CompilerException(string.Format(ErrorMessages.UnexpectedVariable, d.Name));
        }
    }
}
