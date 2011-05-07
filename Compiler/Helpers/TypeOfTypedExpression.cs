using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Types;

namespace Purity.Compiler.Helpers
{
    public class TypeOfTypedExpression : ITypedExpressionVisitor<IType>
    {
        private readonly IMetadataContainer container;

        public TypeOfTypedExpression(IMetadataContainer container)
        {
            this.container = container;
        }

        public static IType TypeOf(ITypedExpression expression, IMetadataContainer container)
        {
            return expression.AcceptVisitor(new TypeOfTypedExpression(container));
        }

        public IType VisitIdentity(TypedExpressions.Identity d)
        {
            return new ArrowType(d.Type, d.Type);
        }

        public IType VisitInl(TypedExpressions.Inl d)
        {
            return new ArrowType(d.LeftType, new SumType(d.LeftType, d.RightType));
        }

        public IType VisitInr(TypedExpressions.Inr d)
        {
            return new ArrowType(d.RightType, new SumType(d.LeftType, d.RightType));
        }

        public IType VisitOutl(TypedExpressions.Outl d)
        {
            return new ArrowType(new ProductType(d.LeftType, d.RightType), d.LeftType);
        }

        public IType VisitOutr(TypedExpressions.Outr d)
        {
            return new ArrowType(new ProductType(d.LeftType, d.RightType), d.RightType);
        }

        public IType VisitSynonym(TypedExpressions.DataSynonym d)
        {
            var resolved = container.ResolveValue(d.Identifier);
            return ReplaceTypeParameters.Replace(resolved.Type, d.TypeParameters);
        }

        public IType VisitAbstraction(TypedExpressions.Abstraction d)
        {
            return new ArrowType(d.VariableType, d.BodyType);
        }

        public IType VisitVariable(TypedExpressions.Variable d)
        {
            return d.Type;
        }

        public IType VisitApplication(TypedExpressions.Application d)
        {
            return (d.LeftType as ArrowType).Right;
        }

        public IType VisitCase(TypedExpressions.Case d)
        {
            return new ArrowType(new SumType(d.LeftType, d.RightType), d.ResultType);
        }

        public IType VisitComposition(TypedExpressions.Composition d)
        {
            return new ArrowType(d.LeftType, d.RightType);
        }

        public IType VisitConst(TypedExpressions.Const d)
        {
            return new ArrowType(d.InputType, d.OutputType);
        }

        public IType VisitSplit(TypedExpressions.Split d)
        {
            return new ArrowType(d.InputType, new ProductType(d.LeftType, d.RightType));
        }

        public IType VisitUncurry(TypedExpressions.Uncurried d)
        {
            return new ArrowType(new ProductType(d.First, d.Second), d.Output);
        }

        public IType VisitCurry(TypedExpressions.Curried d)
        {
            return new ArrowType(d.First, new ArrowType(d.Second, d.Output));
        }
    }
}
