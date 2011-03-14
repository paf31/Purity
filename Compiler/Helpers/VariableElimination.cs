using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Utilities;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Types;

namespace Purity.Compiler.Helpers
{
    public class VariableElimination : ITypedExpressionVisitor
    {
        private IType variableType;
        private string variableName;

        public ITypedExpression Result
        {
            get;
            set;
        }

        private VariableElimination(string variableName, IType variableType)
        {
            this.variableName = variableName;
            this.variableType = variableType;
        }

        public static ITypedExpression Visit(ITypedExpression expression, string variableName, IType variableType)
        {
            var visitor = new VariableElimination(variableName, variableType);
            expression.AcceptVisitor(visitor);
            return visitor.Result;
        }

        public void VisitIn(TypedExpressions.In d)
        {
            var fix = d.Source as IFixedPointType;

            if (fix == null)
            {
                throw new CompilerException(ErrorMessages.ExpectedFixedPointType);
            }

            Result = new TypedExpressions.Const(d,
                variableType,
                new Types.ArrowType(fix, FunctorApplication.Map(fix.Functor, fix)));
        }

        public void VisitOut(TypedExpressions.Out d)
        {
            var fix = d.Target as IFixedPointType;

            if (fix == null)
            {
                throw new CompilerException(ErrorMessages.ExpectedFixedPointType);
            }

            Result = new TypedExpressions.Const(d,
                variableType,
                new Types.ArrowType(FunctorApplication.Map(fix.Functor, fix), fix));
        }

        public void VisitCase(TypedExpressions.Case d)
        {
            var left = Visit(d.Left, variableName, variableType);
            var right = Visit(d.Right, variableName, variableType);

            // [<f, g>] = curry (<uncurry [f], uncurry [g]> . distr)

            Result = new TypedExpressions.Curried
                (
                    new TypedExpressions.Composition
                    (
                        new TypedExpressions.Case
                            (
                                new TypedExpressions.Uncurried
                                (
                                    left,
                                    variableType,
                                    d.LeftType,
                                    d.ResultType
                                ),
                                new TypedExpressions.Uncurried
                                (
                                    right,
                                    variableType,
                                    d.RightType,
                                    d.ResultType
                                ),
                                new Types.ProductType(variableType, d.LeftType),
                                new Types.ProductType(variableType, d.RightType),
                                d.ResultType
                            ),
                        TypedExpressionUtilities.Distr(variableType, d.LeftType, d.RightType),
                        new Types.ProductType(variableType, new Types.SumType(d.LeftType, d.RightType)),
                        new Types.SumType(new Types.ProductType(variableType, d.LeftType), new Types.ProductType(variableType, d.RightType)),
                        d.ResultType
                    ),
                    variableType,
                    new Types.SumType(d.LeftType, d.RightType),
                    d.ResultType
                );
        }

        public void VisitSplit(TypedExpressions.Split d)
        {
            var left = Visit(d.Left, variableName, variableType);
            var right = Visit(d.Right, variableName, variableType);

            // [(f, g)] = curry (uncurry [f], uncurry [g])

            Result = new TypedExpressions.Curried
                (
                    new TypedExpressions.Split
                    (
                        new TypedExpressions.Uncurried(left, variableType, d.InputType, d.LeftType),
                        new TypedExpressions.Uncurried(right, variableType, d.InputType, d.RightType),
                        d.LeftType,
                        d.RightType,
                        new Types.ProductType(variableType, d.InputType)
                    ),
                    variableType,
                    d.InputType,
                    new Types.ProductType(d.LeftType, d.RightType)
                );
        }

        public void VisitApplication(TypedExpressions.Application d)
        {
            var left = Visit(d.Left, variableName, variableType);
            var right = Visit(d.Right, variableName, variableType);

            // [f x] = (uncurry [f]) . (id, [x])

            var domain = (d.LeftType as Types.ArrowType).Left;
            var codomain = (d.LeftType as Types.ArrowType).Right;

            Result = new TypedExpressions.Composition
                (
                    new TypedExpressions.Uncurried(left, variableType, domain, codomain),
                    new TypedExpressions.Split
                    (
                        new TypedExpressions.Identity(variableType),
                        right,
                        variableType,
                        domain,
                        variableType
                    ),
                    variableType,
                    new Types.ProductType(variableType, domain),
                    codomain
                );
        }

        public void VisitComposition(TypedExpressions.Composition d)
        {
            var left = Visit(d.Left, variableName, variableType);
            var right = Visit(d.Right, variableName, variableType);

            // [f . g] = curry ((uncurry [f]) . (outl, uncurry [g]))

            Result = new TypedExpressions.Curried
                (
                    new TypedExpressions.Composition
                    (
                        new TypedExpressions.Uncurried(left, variableType, d.MiddleType, d.RightType),
                        new TypedExpressions.Split
                            (
                                new TypedExpressions.Outl(variableType, d.LeftType),
                                new TypedExpressions.Uncurried(right, variableType, d.LeftType, d.MiddleType),
                                variableType,
                                d.MiddleType,
                                new Types.ProductType(variableType, d.LeftType)
                            ),
                        new Types.ProductType(variableType, d.LeftType),
                        new Types.ProductType(variableType, d.MiddleType),
                        d.RightType
                    ),
                    variableType,
                    d.LeftType,
                    d.RightType
                );
        }

        public void VisitConst(TypedExpressions.Const d)
        {
            var value = Visit(d.Value, variableName, variableType);

            // [const x] = curry ([x] . outl)

            Result = new TypedExpressions.Curried
                (
                    new TypedExpressions.Composition
                    (
                        value,
                        new TypedExpressions.Outl(variableType, d.InputType),
                        new Types.ProductType(variableType, d.InputType),
                        variableType,
                        d.OutputType
                    ),
                    variableType,
                    d.InputType,
                    d.OutputType
                );
        }

        public void VisitUncurry(TypedExpressions.Uncurried d)
        {
            var function = Visit(d.Function, variableName, variableType);

            // [curry f] = curry (uncurry uncurry [f] . assocl)

            Result = new TypedExpressions.Curried
                (
                    new TypedExpressions.Composition
                    (
                        new TypedExpressions.Uncurried
                            (
                                new TypedExpressions.Uncurried
                                    (
                                        function,
                                        variableType,
                                        d.First,
                                        new Types.ArrowType(d.Second, d.Output)
                                    ),
                                new Types.ProductType(variableType, d.First),
                                d.Second,
                                d.Output
                            ),
                        TypedExpressionUtilities.Assocl(variableType, d.First, d.Second),
                        new Types.ProductType(variableType, new Types.ProductType(d.First, d.Second)),
                        new Types.ProductType(new Types.ProductType(variableType, d.First), d.Second),
                        d.Output
                    ),
                    variableType,
                    new Types.ProductType(d.First, d.Second),
                    d.Output
                );
        }

        public void VisitCurry(TypedExpressions.Curried d)
        {
            var function = Visit(d.Function, variableName, variableType);

            // [curry f] = curry curry (uncurry [f] . assocr)

            Result = new TypedExpressions.Curried
                (
                    new TypedExpressions.Curried
                        (
                            new TypedExpressions.Composition
                            (
                                new TypedExpressions.Uncurried
                                    (
                                        function,
                                        variableType,
                                        new Types.ProductType(d.First, d.Second),
                                        d.Output
                                    ),
                                    TypedExpressionUtilities.Assocr(variableType, d.First, d.Second),
                                    new Types.ProductType(new Types.ProductType(variableType, d.First), d.Second),
                                    new Types.ProductType(variableType, new Types.ProductType(d.First, d.Second)),
                                    d.Output
                            ),
                            new Types.ProductType(variableType, d.First),
                            d.Second,
                            d.Output
                        ),
                    variableType,
                    d.First,
                    new Types.ArrowType(d.Second, d.Output)
                );
        }

        public void VisitAbstraction(TypedExpressions.Abstraction d)
        {
            d.PointFreeExpression.AcceptVisitor(this);
        }

        public void VisitVariable(TypedExpressions.Variable d)
        {
            if (d.Name.Equals(variableName))
            {
                Result = new TypedExpressions.Identity(d.Type);
            }
            else
            {
                Result = new TypedExpressions.Const(d, variableType, d.Type);
            }
        }

        public void VisitIdentity(TypedExpressions.Identity d)
        {
            Result = new TypedExpressions.Const(d,
                variableType, new Types.ArrowType(d.Type, d.Type));
        }

        public void VisitInl(TypedExpressions.Inl d)
        {
            Result = new TypedExpressions.Const(d,
                variableType, new Types.ArrowType(d.LeftType, new Types.SumType(d.LeftType, d.RightType)));
        }

        public void VisitInr(TypedExpressions.Inr d)
        {
            Result = new TypedExpressions.Const(d,
                 variableType, new Types.ArrowType(d.RightType, new Types.SumType(d.LeftType, d.RightType)));
        }

        public void VisitOutl(TypedExpressions.Outl d)
        {
            Result = new TypedExpressions.Const(d,
                variableType, new Types.ArrowType(new Types.ProductType(d.LeftType, d.RightType), d.LeftType));
        }

        public void VisitOutr(TypedExpressions.Outr d)
        {
            Result = new TypedExpressions.Const(d,
                 variableType, new Types.ArrowType(new Types.ProductType(d.LeftType, d.RightType), d.RightType));
        }

        public void VisitSynonym(TypedExpressions.DataSynonym d)
        {
            Result = new TypedExpressions.Const(d, variableType, Container.ResolveValue(d.Identifier).Type);
        }

        public void VisitBox(TypedExpressions.Box d)
        {
            Result = new TypedExpressions.Const(d, variableType, new Types.ArrowType(d.Target, d.Type));
        }

        public void VisitUnbox(TypedExpressions.Unbox d)
        {
            Result = new TypedExpressions.Const(d, variableType, new Types.ArrowType(d.Type, d.Target));
        }

        public void VisitAna(TypedExpressions.Ana d)
        {
            Result = new TypedExpressions.Const(d,
                variableType, new Types.ArrowType(new Types.ArrowType(
                    d.CarrierType, FunctorApplication.Map((d.GFixType as GFixType).Functor, d.CarrierType)), 
                    new Types.ArrowType(d.CarrierType, d.GFixType)));
        }

        public void VisitCata(TypedExpressions.Cata d)
        {
            Result = new TypedExpressions.Const(d,
                variableType, new Types.ArrowType(new Types.ArrowType(
                    FunctorApplication.Map((d.LFixType as LFixType).Functor, d.CarrierType), d.CarrierType), 
                    new Types.ArrowType(d.LFixType, d.CarrierType)));
        }
    }
}
