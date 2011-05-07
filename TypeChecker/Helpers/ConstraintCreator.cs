using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Data;
using Purity.Compiler.Typechecker.Types;
using Purity.Compiler.Typechecker.Classes;
using Purity.Compiler.Typechecker.Utilities;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class ConstraintCreator : IDataVisitor<TypeCheckingResult>
    {
        private readonly TypeCheckingContext context;
        private readonly TypeCheckingEnvironment environment;
        private readonly IMetadataContainer container;

        public ConstraintCreator(TypeCheckingEnvironment environment, TypeCheckingContext context, IMetadataContainer container)
        {
            this.environment = environment;
            this.context = context;
            this.container = container;
        }

        public static TypeCheckingResult CreateConstraints(TypeCheckingEnvironment environment, TypeCheckingContext context, IData expression, IMetadataContainer container)
        {
            return expression.AcceptVisitor(new ConstraintCreator(environment, context, container));
        }

        public static TypeCheckingResult W(IData data, IMetadataContainer container)
        {
            return CreateConstraints(Environments.Empty(), new TypeCheckingContext(), data, container);
        }

        public TypeCheckingResult VisitApplication(Application d)
        {
            var w1 = CreateConstraints(environment, context, d.Left, container);
            var w2 = CreateConstraints(environment, context, d.Right, container);

            var resultType = context.NewType();

            var s1 = w1.Constraints.Concat(w2.Constraints);

            var arrow = new Types.ArrowType(w2.Type, resultType);

            var s2 = Unification.Unify(w1.Type, arrow);
            var s3 = s1.Concat(s2);

            return new TypeCheckingResult(new Data.Application(w1.Data, w2.Data, w1.Type, w2.Type), resultType, s3, context.Index);
        }

        public TypeCheckingResult VisitCase(Case d)
        {
            var w1 = CreateConstraints(environment, context, d.Left, container);
            var w2 = CreateConstraints(environment, context, d.Right, container);

            var left = context.NewType();
            var right = context.NewType();
            var result = context.NewType();

            var leftArrow = new ArrowType(left, result);
            var rightArrow = new ArrowType(right, result);
            var caseArrow = new ArrowType(new SumType(left, right), result);

            var s1 = Unification.Unify(leftArrow, w1.Type);
            var s2 = Unification.Unify(rightArrow, w2.Type);

            var s = w1.Constraints.Concat(w2.Constraints).Concat(s1).Concat(s2);

            return new TypeCheckingResult(new Data.Case(w1.Data, w2.Data, left, right, result), caseArrow, s, context.Index);
        }

        public TypeCheckingResult VisitComposition(Composition d)
        {
            var first = context.NewType();
            var second = context.NewType();
            var third = context.NewType();

            var composition = new Types.ArrowType(first, third);

            var w1 = CreateConstraints(environment, context, d.Left, container);
            var w2 = CreateConstraints(environment, context, d.Right, container);

            var s1 = Unification.Unify(w1.Type, new Types.ArrowType(second, third));
            var s2 = Unification.Unify(w2.Type, new Types.ArrowType(first, second));

            var s = w1.Constraints.Concat(w2.Constraints).Concat(s1).Concat(s2);

            return new TypeCheckingResult(new Data.Composition(w1.Data, w2.Data, first, second, third), composition, s, context.Index);
        }

        public TypeCheckingResult VisitConst(Const d)
        {
            var w = CreateConstraints(environment, context, d.Value, container);

            var inputType = context.NewType();

            return new TypeCheckingResult(new Data.Const(w.Data, inputType, w.Type), new ArrowType(inputType, w.Type), w.Constraints, context.Index);
        }

        public TypeCheckingResult VisitIdentity(Identity d)
        {
            var type = context.NewType();
            return new TypeCheckingResult(new Data.Identity(type), new ArrowType(type, type), Constraints.Identity(), context.Index);
        }

        public TypeCheckingResult VisitInl(Inl d)
        {
            var leftType = context.NewType();
            var rightType = context.NewType();
            var type = new ArrowType(leftType, new SumType(leftType, rightType));
            return new TypeCheckingResult(new Data.Inl(leftType, rightType), type, Constraints.Identity(), context.Index);
        }

        public TypeCheckingResult VisitInr(Inr d)
        {
            var leftType = context.NewType();
            var rightType = context.NewType();
            var type = new ArrowType(rightType, new SumType(leftType, rightType));
            return new TypeCheckingResult(new Data.Inr(leftType, rightType), type, Constraints.Identity(), context.Index);
        }

        public TypeCheckingResult VisitOutl(Outl d)
        {
            var leftType = context.NewType();
            var rightType = context.NewType();
            var type = new ArrowType(new ProductType(leftType, rightType), leftType);
            return new TypeCheckingResult(new Data.Outl(leftType, rightType), type, Constraints.Identity(), context.Index);
        }

        public TypeCheckingResult VisitOutr(Outr d)
        {
            var leftType = context.NewType();
            var rightType = context.NewType();
            var type = new ArrowType(new ProductType(leftType, rightType), rightType);
            return new TypeCheckingResult(new Data.Outr(leftType, rightType), type, Constraints.Identity(), context.Index);
        }

        public TypeCheckingResult VisitSplit(Split d)
        {
            var w1 = CreateConstraints(environment, context, d.Left, container);
            var w2 = CreateConstraints(environment, context, d.Right, container);

            var inputType = context.NewType();
            var leftType = context.NewType();
            var rightType = context.NewType();

            var leftArrow = new ArrowType(inputType, leftType);
            var rightArrow = new ArrowType(inputType, rightType);
            var splitArrow = new ArrowType(inputType, new ProductType(leftType, rightType));

            var s1 = Unification.Unify(leftArrow, w1.Type);
            var s2 = Unification.Unify(rightArrow, w2.Type);

            var s = w1.Constraints.Concat(w2.Constraints).Concat(s1).Concat(s2);

            return new TypeCheckingResult(new Data.Split(w1.Data, w2.Data, leftType, rightType, inputType), splitArrow, s, context.Index);
        }

        public TypeCheckingResult VisitUncurry(Uncurried d)
        {
            var w1 = CreateConstraints(environment, context, d.Function, container);

            var first = context.NewType();
            var second = context.NewType();
            var result = context.NewType();

            var uncurried = new ArrowType(new ProductType(first, second), result);
            var curried = new ArrowType(first, new ArrowType(second, result));

            var s1 = Unification.Unify(curried, w1.Type);
            var s = w1.Constraints.Concat(s1);

            return new TypeCheckingResult(new Data.Uncurried(w1.Data, first, second, result), uncurried, s, context.Index);
        }

        public TypeCheckingResult VisitCurry(Curried d)
        {
            var w1 = CreateConstraints(environment, context, d.Function, container);

            var first = context.NewType();
            var second = context.NewType();
            var result = context.NewType();

            var uncurried = new ArrowType(new ProductType(first, second), result);
            var curried = new ArrowType(first, new ArrowType(second, result));

            var s1 = Unification.Unify(uncurried, w1.Type);
            var s = w1.Constraints.Concat(s1);

            return new TypeCheckingResult(new Data.Curried(w1.Data, first, second, result), curried, s, context.Index);
        }

        public TypeCheckingResult VisitSynonym(DataSynonym d)
        {
            var declaration = container.ResolveValue(d.Identifier);

            var typeParameters = new Dictionary<string, IPartialType>();

            for (int i = 0; i < declaration.TypeParameters.Length; i++)
            {
                var parameterName = declaration.TypeParameters[i];
                var parameterType = context.NewType();
                typeParameters.Add(parameterName, parameterType);
            }

            var type = PartialTypeCreator.Convert(declaration.Type, ident => typeParameters[ident]);

            return new TypeCheckingResult(new Data.DataSynonym(d.Identifier, typeParameters), type, Constraints.Identity(), context.Index);
        }

        public TypeCheckingResult VisitAbstraction(Abstraction d)
        {
            var variableType = context.NewType();
            var newEnv = environment.Replace(d.Variable, variableType);

            var w1 = CreateConstraints(newEnv, context, d.Body, container);

            var arrowType = new Types.ArrowType(variableType, w1.Type);

            return new TypeCheckingResult(new Data.Abstraction(d.Variable, w1.Data, variableType, w1.Type), arrowType, w1.Constraints, context.Index);
        }

        public TypeCheckingResult VisitVariable(Variable d)
        {
            var variableType = environment(d.Name);

            return new TypeCheckingResult(new Data.Variable(d.Name, variableType), variableType, Constraints.Identity(), context.Index);
        }
    }
}
