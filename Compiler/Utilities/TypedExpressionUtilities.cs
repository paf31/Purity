using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Utilities
{
    public static class TypedExpressionUtilities
    {
        public static ITypedExpression Assocl(IType a, IType b, IType c)
        {
            return new TypedExpressions.Split
                (
                    new TypedExpressions.Split
                        (
                            new TypedExpressions.Outl(a, new Types.ProductType(b, c)),
                            new TypedExpressions.Composition
                                (
                                    new TypedExpressions.Outl(b, c),
                                    new TypedExpressions.Outr(a, new Types.ProductType(b, c)),
                                    new Types.ProductType(a, new Types.ProductType(b, c)),
                                    new Types.ProductType(b, c),
                                    b
                                ),
                            a,
                            b,
                            new Types.ProductType(a, new Types.ProductType(b, c))
                        ),
                    new TypedExpressions.Composition
                        (
                            new TypedExpressions.Outr(b, c),
                            new TypedExpressions.Outr(a, new Types.ProductType(b, c)),
                            new Types.ProductType(a, new Types.ProductType(b, c)),
                            new Types.ProductType(b, c),
                            c
                        ),
                    new Types.ProductType(a, b),
                    c,
                    new Types.ProductType(a, new Types.ProductType(b, c))
                );
        }

        public static ITypedExpression Assocr(IType a, IType b, IType c)
        {
            return new TypedExpressions.Split
                (
                    new TypedExpressions.Composition
                        (
                            new TypedExpressions.Outl(a, b),
                            new TypedExpressions.Outl(new Types.ProductType(a, b), c),
                            new Types.ProductType(new Types.ProductType(a, b), c),
                            new Types.ProductType(a, b),
                            a
                        ),
                    new TypedExpressions.Split
                        (
                            new TypedExpressions.Composition
                                (
                                    new TypedExpressions.Outr(a, b),
                                    new TypedExpressions.Outl(new Types.ProductType(a, b), c),
                                    new Types.ProductType(new Types.ProductType(a, b), c),
                                    new Types.ProductType(a, b),
                                    b
                                ),
                            new TypedExpressions.Outr(new Types.ProductType(a, b), c),
                            b,
                            c,
                            new Types.ProductType(new Types.ProductType(a, b), c)
                        ),
                    a,
                    new Types.ProductType(b, c),
                    new Types.ProductType(new Types.ProductType(a, b), c)
                );
        }

        public static ITypedExpression Distl(IType a, IType b, IType c)
        {
            return new TypedExpressions.Uncurried
                (
                    new TypedExpressions.Case
                    (
                        new TypedExpressions.Curried
                            (

                                new TypedExpressions.Inl(new Types.ProductType(a, c), new Types.ProductType(b, c)),
                                a,
                                c,
                                new Types.SumType(new Types.ProductType(a, c), new Types.ProductType(b, c))
                            ),
                        new TypedExpressions.Curried
                            (
                                new TypedExpressions.Inr(new Types.ProductType(a, c), new Types.ProductType(b, c)),
                                b,
                                c,
                                new Types.SumType(new Types.ProductType(a, c), new Types.ProductType(b, c))
                            ),
                        a,
                        b,
                        new Types.ArrowType(c, new Types.SumType(new Types.ProductType(a, c), new Types.ProductType(b, c)))
                    ),
                    new Types.SumType(a, b),
                    c,
                    new Types.SumType(new Types.ProductType(a, c), new Types.ProductType(b, c))
                );
        }

        public static ITypedExpression Distr(IType a, IType b, IType c)
        {
            return new TypedExpressions.Composition
                (
                    Sum(
                        Flip(b, a),
                        Flip(c, a),
                        new Types.ProductType(b, a),
                        new Types.ProductType(a, b),
                        new Types.ProductType(c, a),
                        new Types.ProductType(a, c)
                    ),
                    new TypedExpressions.Composition
                    (
                        Distl(b, c, a),
                        Flip(a, new Types.SumType(b, c)),
                        new Types.ProductType(a, new Types.SumType(b, c)),
                        new Types.ProductType(new Types.SumType(b, c), a),
                        new Types.SumType(new Types.ProductType(b, a), new Types.ProductType(c, a))
                    ),
                    new Types.ProductType(a, new Types.SumType(b, c)),
                    new Types.SumType(new Types.ProductType(b, a), new Types.ProductType(c, a)),
                    new Types.SumType(new Types.ProductType(a, b), new Types.ProductType(a, c))
                );
        }

        public static ITypedExpression Sum(ITypedExpression left, ITypedExpression right,
            IType l1, IType l2, IType r1, IType r2)
        {
            return new TypedExpressions.Case
                (
                    new TypedExpressions.Composition(new TypedExpressions.Inl(l2, r2), left, l1, l2, new Types.SumType(l2, r2)),
                    new TypedExpressions.Composition(new TypedExpressions.Inr(l2, r2), right, r1, r2, new Types.SumType(l2, r2)),
                    l1,
                    r1,
                    new Types.SumType(l2, r2)
                );
        }

        public static ITypedExpression Flip(IType a, IType b)
        {
            return new TypedExpressions.Split
                (
                    new TypedExpressions.Outr(a, b),
                    new TypedExpressions.Outl(a, b),
                    b, 
                    a,
                    new Types.ProductType(a, b)
                );
        }
    }
}
