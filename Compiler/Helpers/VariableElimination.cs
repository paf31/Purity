using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Utilities;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Types;
using Purity.Compiler.Typechecker.Helpers;

namespace Purity.Compiler.Helpers
{
    public class VariableElimination : ITypedExpressionVisitor<ITypedExpression>
    {
        private IType variableType;
        private string variableName;

        private VariableElimination(string variableName, IType variableType)
        {
            this.variableName = variableName;
            this.variableType = variableType;
        }

        public static ITypedExpression Visit(ITypedExpression expression, string variableName, IType variableType)
        {
            /**
             * TODO: In the worst case, we make this check for every structurally smaller 
             * value in the expression tree, but if it is true for the root then it is true
             * for all structurally smaller values too.
             */
            if (ConstantExpressions.IsConstantExpression(expression, variableName)) 
            {
                return Constant(expression, variableType);
            }
            else
            {
                var visitor = new VariableElimination(variableName, variableType);
                return expression.AcceptVisitor(visitor);
            }
        }

        private static ITypedExpression Constant(ITypedExpression expression, IType variableType)
        {
            var type = TypeOfTypedExpression.TypeOf(expression);
            return new TypedExpressions.Const(expression, variableType, type);
        }

        public ITypedExpression VisitCase(TypedExpressions.Case d)
        {
            var left = Visit(d.Left, variableName, variableType);
            var right = Visit(d.Right, variableName, variableType);

            // [<f, g>] = curry (<uncurry [f], uncurry [g]> . distr)

            return new TypedExpressions.Curried
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

        public ITypedExpression VisitSplit(TypedExpressions.Split d)
        {
            var left = Visit(d.Left, variableName, variableType);
            var right = Visit(d.Right, variableName, variableType);

            // [(f, g)] = curry (uncurry [f], uncurry [g])

            return new TypedExpressions.Curried
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

        public ITypedExpression VisitApplication(TypedExpressions.Application d)
        {
            var left = Visit(d.Left, variableName, variableType);
            var right = Visit(d.Right, variableName, variableType);

            // [f x] = (uncurry [f]) . (id, [x])

            var domain = (d.LeftType as Types.ArrowType).Left;
            var codomain = (d.LeftType as Types.ArrowType).Right;

            return new TypedExpressions.Composition
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

        public ITypedExpression VisitComposition(TypedExpressions.Composition d)
        {
            var left = Visit(d.Left, variableName, variableType);
            var right = Visit(d.Right, variableName, variableType);

            // [f . g] = curry ((uncurry [f]) . (outl, uncurry [g]))

            return new TypedExpressions.Curried
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

        public ITypedExpression VisitConst(TypedExpressions.Const d)
        {
            var value = Visit(d.Value, variableName, variableType);

            // [const x] = curry ([x] . outl)

            return new TypedExpressions.Curried
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

        public ITypedExpression VisitUncurry(TypedExpressions.Uncurried d)
        {
            var function = Visit(d.Function, variableName, variableType);

            // [curry f] = curry (uncurry uncurry [f] . assocl)

            return new TypedExpressions.Curried
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

        public ITypedExpression VisitCurry(TypedExpressions.Curried d)
        {
            var function = Visit(d.Function, variableName, variableType);

            // [curry f] = curry curry (uncurry [f] . assocr)

            return new TypedExpressions.Curried
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

        public ITypedExpression VisitVariable(TypedExpressions.Variable d)
        {
            if (d.Name.Equals(variableName))
            {
                return new TypedExpressions.Identity(d.Type);
            }
            else
            {
                return Constant(d, variableType);
            }
        }

        public ITypedExpression VisitAbstraction(TypedExpressions.Abstraction d)
        {
            return d.PointFreeExpression.AcceptVisitor(this);
        }

        public ITypedExpression VisitIdentity(TypedExpressions.Identity d)
        {
            return Constant(d, variableType);
        }

        public ITypedExpression VisitInl(TypedExpressions.Inl d)
        {
            return Constant(d, variableType);
        }

        public ITypedExpression VisitInr(TypedExpressions.Inr d)
        {
            return Constant(d, variableType);
        }

        public ITypedExpression VisitOutl(TypedExpressions.Outl d)
        {
            return Constant(d, variableType);
        }

        public ITypedExpression VisitOutr(TypedExpressions.Outr d)
        {
            return Constant(d, variableType);
        }

        public ITypedExpression VisitSynonym(TypedExpressions.DataSynonym d)
        {
            return Constant(d, variableType);
        }
    }
}
