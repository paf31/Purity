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

        public DataCompiler(ILGenerator body)
        {
            this.body = body;
        }

        public void VisitAna(TypedExpressions.Ana d)
        {
            var gfix = d.GFixType as GFixType;

            if (gfix == null)
            {
                throw new CompilerException(ErrorMessages.ExpectedGFixType);
            }

            var typeInfo = TypeContainer.ResolveGFixType(gfix.Identifier);
            var anaClass = typeInfo.AnaFunction1;
            var genericCtor = typeInfo.AnaFunction1Constructor;
            var genericParameter = new TypeConverter().Convert(d.CarrierType);
            var ctor = TypeBuilder.GetConstructor(anaClass.MakeGenericType(genericParameter), genericCtor);
            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitCata(TypedExpressions.Cata d)
        {
            var lfix = d.LFixType as LFixType;

            if (lfix == null)
            {
                throw new CompilerException(ErrorMessages.ExpectedLFixType);
            }

            var typeInfo = TypeContainer.ResolveLFixType(lfix.Identifier);
            var cataClass = typeInfo.CataFunction1;
            var genericCtor = typeInfo.CataFunction1Constructor;
            var genericParameter = new TypeConverter().Convert(d.CarrierType);
            var ctor = TypeBuilder.GetConstructor(cataClass.MakeGenericType(genericParameter), genericCtor);
            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitIn(TypedExpressions.In d)
        {
            var source = d.Source;

            var fix = source as IFixedPointType;

            if (fix == null)
            {
                throw new CompilerException(ErrorMessages.ExpectedFixedPointType);
            }

            var ctor = TypeContainer.ResolveFixPointType(fix.Identifier).InFunctionConstructor;
            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitOut(TypedExpressions.Out d)
        {
            var target = d.Target;

            var fix = target as IFixedPointType;

            if (fix == null)
            {
                throw new CompilerException(ErrorMessages.ExpectedFixedPointType);
            }

            var ctor = TypeContainer.ResolveFixPointType(fix.Identifier).OutFunctionConstructor;
            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitApplication(TypedExpressions.Application d)
        {
            d.Left.AcceptVisitor(this);
            d.Right.AcceptVisitor(this);

            var functionType = new TypeConverter().Convert(d.LeftType);
            var callMethod = typeof(IFunction<,>).GetMethod(Constants.CallMethodName);

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

        public void VisitSynonym(TypedExpressions.DataSynonym d)
        {
            var method = DataContainer.Resolve(d.Identifier).Method;
            body.Emit(OpCodes.Call, method);
        }

        public void VisitBox(TypedExpressions.Box d)
        {
            var synonym = d.Type as TypeSynonym;

            if (synonym == null)
            {
                throw new CompilerException(ErrorMessages.ExpectedTypeSynonym);
            }

            var type = (BoxedTypeInfo)TypeContainer.ResolveType(synonym.Identifier);
            var ctor = type.BoxFunctionConstructor;
            body.Emit(OpCodes.Newobj, ctor);
        }

        public void VisitUnbox(TypedExpressions.Unbox d)
        {
            var synonym = d.Type as TypeSynonym;

            if (synonym == null)
            {
                throw new CompilerException(ErrorMessages.ExpectedTypeSynonym);
            }

            var type = (BoxedTypeInfo)TypeContainer.ResolveType(synonym.Identifier);
            var ctor = type.UnboxFunctionConstructor;
            body.Emit(OpCodes.Newobj, ctor);
        }

        public static void CompileMethod(string name, TypeBuilder dataClass, ITypedExpression typedExpression,
            DataDeclaration data)
        {
            var converter = new TypeConverter();

            var converted = converter.Convert(data.Type);
            var method = dataClass.DefineMethod(name,
                MethodAttributes.Public | MethodAttributes.Static, converted, Type.EmptyTypes);
            var body = method.GetILGenerator();
            typedExpression.AcceptVisitor(new DataCompiler(body));
            body.Emit(OpCodes.Ret);

            Container.Add(name, data);

            var dataInfo = new DataInfo();
            dataInfo.Method = method;
            DataContainer.Add(name, dataInfo);

            var arguments = new List<IType>();
            var returnType = data.Type;

            while (returnType is ArrowType)
            {
                var arrow = returnType as ArrowType;

                arguments.Add(arrow.Left);
                returnType = arrow.Right;

                var lastArgument = converter.Convert(arrow.Left);
                var compiledReturnType = converter.Convert(returnType);

                var curried = dataClass.DefineMethod(name,
                    MethodAttributes.Public | MethodAttributes.Static,
                    compiledReturnType,
                    arguments.Select(converter.Convert).ToArray());
                var curriedBody = curried.GetILGenerator();

                for (int argIndex = 0; argIndex < arguments.Count - 1; argIndex++)
                {
                    curriedBody.Emit(OpCodes.Ldarg, argIndex);
                }

                curriedBody.Emit(OpCodes.Call, method);
                curriedBody.Emit(OpCodes.Ldarg, arguments.Count - 1);
                curriedBody.Emit(OpCodes.Callvirt, TypeBuilder.GetMethod(
                    typeof(IFunction<,>).MakeGenericType(lastArgument, compiledReturnType),
                    typeof(IFunction<,>).GetMethod(Constants.CallMethodName)));
                curriedBody.Emit(OpCodes.Ret);

                method = curried;
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
