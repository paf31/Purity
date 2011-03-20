using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class TypedExpressionCreator : IConstrainedDataVisitor<ITypedExpression>
    {
        public static ITypedExpression Convert(IConstrainedData data)
        {
            return data.AcceptVisitor(new TypedExpressionCreator());
        }

        public ITypedExpression VisitAna(Data.Ana d)
        {
            return new Purity.Compiler.TypedExpressions.Ana(
                TypeConverter.Convert(d.CarrierType),
                TypeConverter.Convert(d.GFixType));
        }

        public ITypedExpression VisitApplication(Data.Application d)
        {
            return new Purity.Compiler.TypedExpressions.Application(
                Convert(d.Left),
                Convert(d.Right),
                TypeConverter.Convert(d.LeftType),
                TypeConverter.Convert(d.RightType));
        }

        public ITypedExpression VisitCase(Data.Case d)
        {
            return new Purity.Compiler.TypedExpressions.Case(
                Convert(d.Left),
                Convert(d.Right),
                TypeConverter.Convert(d.LeftType),
                TypeConverter.Convert(d.RightType),
                TypeConverter.Convert(d.ResultType));
        }

        public ITypedExpression VisitCata(Data.Cata d)
        {
            return new Purity.Compiler.TypedExpressions.Cata(
                TypeConverter.Convert(d.CarrierType),
                TypeConverter.Convert(d.LFixType));
        }

        public ITypedExpression VisitComposition(Data.Composition d)
        {
            return new Purity.Compiler.TypedExpressions.Composition(
                Convert(d.Left),
                Convert(d.Right),
                TypeConverter.Convert(d.LeftType),
                TypeConverter.Convert(d.MiddleType),
                TypeConverter.Convert(d.RightType));
        }

        public ITypedExpression VisitConst(Data.Const d)
        {
            return new Purity.Compiler.TypedExpressions.Const(
                Convert(d.Value),
                TypeConverter.Convert(d.InputType),
                TypeConverter.Convert(d.OutputType));
        }

        public ITypedExpression VisitIdentity(Data.Identity d)
        {
            return new Purity.Compiler.TypedExpressions.Identity(
                TypeConverter.Convert(d.Type));
        }

        public ITypedExpression VisitInl(Data.Inl d)
        {
            return new Purity.Compiler.TypedExpressions.Inl(
                TypeConverter.Convert(d.LeftType),
                TypeConverter.Convert(d.RightType));
        }

        public ITypedExpression VisitInr(Data.Inr d)
        {
            return new Purity.Compiler.TypedExpressions.Inr(
                TypeConverter.Convert(d.LeftType),
                TypeConverter.Convert(d.RightType));
        }

        public ITypedExpression VisitOutl(Data.Outl d)
        {
            return new Purity.Compiler.TypedExpressions.Outl(
                TypeConverter.Convert(d.LeftType),
                TypeConverter.Convert(d.RightType));
        }

        public ITypedExpression VisitOutr(Data.Outr d)
        {
            return new Purity.Compiler.TypedExpressions.Outr(
                TypeConverter.Convert(d.LeftType),
                TypeConverter.Convert(d.RightType));
        }

        public ITypedExpression VisitSplit(Data.Split d)
        {
            return new Purity.Compiler.TypedExpressions.Split(
                Convert(d.Left),
                Convert(d.Right),
                TypeConverter.Convert(d.LeftType),
                TypeConverter.Convert(d.RightType),
                TypeConverter.Convert(d.InputType));
        }

        public ITypedExpression VisitIn(Data.In d)
        {
            return new Purity.Compiler.TypedExpressions.In(
                TypeConverter.Convert(d.Source));
        }

        public ITypedExpression VisitOut(Data.Out d)
        {
            return new Purity.Compiler.TypedExpressions.Out(
                TypeConverter.Convert(d.Target));
        }

        public ITypedExpression VisitCurry(Data.Curried d)
        {
            return new Purity.Compiler.TypedExpressions.Curried(
                Convert(d.Function),
                TypeConverter.Convert(d.First),
                TypeConverter.Convert(d.Second),
                TypeConverter.Convert(d.Output));
        }

        public ITypedExpression VisitUncurry(Data.Uncurried d)
        {
            return new Purity.Compiler.TypedExpressions.Uncurried(
                Convert(d.Function),
                TypeConverter.Convert(d.First),
                TypeConverter.Convert(d.Second),
                TypeConverter.Convert(d.Output));
        }

        public ITypedExpression VisitSynonym(Data.DataSynonym d)
        {
            IDictionary<string, IType> typeParameters = new Dictionary<string, IType>();

            foreach (var pair in d.TypeParameters) 
            {
                typeParameters[pair.Key] = TypeConverter.Convert(pair.Value);
            }

            return new Purity.Compiler.TypedExpressions.DataSynonym(d.Identifier,
                typeParameters);
        }

        public ITypedExpression VisitBox(Data.Box d)
        {
            return new Purity.Compiler.TypedExpressions.Box(
                TypeConverter.Convert(d.Target), 
                TypeConverter.Convert(d.Type));
        }

        public ITypedExpression VisitUnbox(Data.Unbox d)
        {
            return new Purity.Compiler.TypedExpressions.Unbox(
                TypeConverter.Convert(d.Target), 
                TypeConverter.Convert(d.Type));
        }

        public ITypedExpression VisitAbstraction(Data.Abstraction d)
        {
            return new Purity.Compiler.TypedExpressions.Abstraction(
                d.Variable,
                Convert(d.Body), 
                TypeConverter.Convert(d.BodyType),
                TypeConverter.Convert(d.VariableType));
        }

        public ITypedExpression VisitVariable(Data.Variable d)
        {
            return new Purity.Compiler.TypedExpressions.Variable(
                d.Name,
                TypeConverter.Convert(d.Type));
        }
    }
}
