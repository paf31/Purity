using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Classes;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Typechecker.Data;
using Purity.Compiler.Typechecker.Types;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class ApplySolutionSet : IConstrainedDataVisitor<IConstrainedData>, IPartialTypeVisitor<IPartialType>
    {
        private readonly SolutionSet solutionSet;

        public ApplySolutionSet(SolutionSet solutionSet)
        {
            this.solutionSet = solutionSet;
        }

        public static IPartialType Apply(SolutionSet solutionSet, IPartialType type)
        {
            ApplySolutionSet visitor = new ApplySolutionSet(solutionSet);
            return type.AcceptVisitor(visitor);
        }

        public static IConstrainedData Apply(SolutionSet solutionSet, IConstrainedData data)
        {
            ApplySolutionSet visitor = new ApplySolutionSet(solutionSet);
            return data.AcceptVisitor(visitor);
        }

        public IConstrainedData VisitApplication(Data.Application d)
        {
            return new Application(
                d.Left.AcceptVisitor(this), 
                d.Right.AcceptVisitor(this), 
                d.LeftType.AcceptVisitor(this), 
                d.RightType.AcceptVisitor(this));
        }

        public IConstrainedData VisitCase(Data.Case d)
        {
            return new Case(
                d.Left.AcceptVisitor(this),
                d.Right.AcceptVisitor(this),
                d.LeftType.AcceptVisitor(this),
                d.RightType.AcceptVisitor(this),
                d.ResultType.AcceptVisitor(this));
        }

        public IConstrainedData VisitComposition(Data.Composition d)
        {
            return new Composition(
                d.Left.AcceptVisitor(this),
                d.Right.AcceptVisitor(this),
                d.LeftType.AcceptVisitor(this),
                d.MiddleType.AcceptVisitor(this),
                d.RightType.AcceptVisitor(this));
        }

        public IConstrainedData VisitConst(Data.Const d)
        {
            return new Const(
                d.Value.AcceptVisitor(this),
                d.InputType.AcceptVisitor(this),
                d.OutputType.AcceptVisitor(this));
        }

        public IConstrainedData VisitIdentity(Data.Identity d)
        {
            return new Identity(d.Type.AcceptVisitor(this));
        }

        public IConstrainedData VisitInl(Data.Inl d)
        {
            return new Inl(
                d.LeftType.AcceptVisitor(this),
                d.RightType.AcceptVisitor(this));
        }

        public IConstrainedData VisitInr(Data.Inr d)
        {
            return new Inr(
                d.LeftType.AcceptVisitor(this),
                d.RightType.AcceptVisitor(this));
        }

        public IConstrainedData VisitOutl(Data.Outl d)
        {
            return new Outl(
                d.LeftType.AcceptVisitor(this),
                d.RightType.AcceptVisitor(this));
        }

        public IConstrainedData VisitOutr(Data.Outr d)
        {
            return new Outr(
                d.LeftType.AcceptVisitor(this),
                d.RightType.AcceptVisitor(this));
        }

        public IConstrainedData VisitSplit(Data.Split d)
        {
            return new Split(
                d.Left.AcceptVisitor(this),
                d.Right.AcceptVisitor(this),
                d.LeftType.AcceptVisitor(this),
                d.RightType.AcceptVisitor(this),
                d.InputType.AcceptVisitor(this));
        }

        public IConstrainedData VisitCurry(Data.Curried d)
        {
            return new Curried(
                d.Function.AcceptVisitor(this),
                d.First.AcceptVisitor(this),
                d.Second.AcceptVisitor(this),
                d.Output.AcceptVisitor(this));
        }

        public IConstrainedData VisitUncurry(Data.Uncurried d)
        {
            return new Uncurried(
                d.Function.AcceptVisitor(this),
                d.First.AcceptVisitor(this),
                d.Second.AcceptVisitor(this),
                d.Output.AcceptVisitor(this));
        }

        public IConstrainedData VisitSynonym(Data.DataSynonym d)
        {
            return new DataSynonym(
                d.Identifier,
                d.TypeParameters.ToDictionary(k => k.Key, k => k.Value.AcceptVisitor(this)));
        }

        public IConstrainedData VisitAbstraction(Data.Abstraction d)
        {
            return new Abstraction(
                d.Variable, 
                d.Body.AcceptVisitor(this), 
                d.VariableType.AcceptVisitor(this), 
                d.BodyType.AcceptVisitor(this));
        }

        public IConstrainedData VisitVariable(Data.Variable d)
        {
            return new Variable(
                d.Name,
                d.Type.AcceptVisitor(this));
        }

        public IPartialType VisitArrow(Types.ArrowType t)
        {
            return new ArrowType(t.Left.AcceptVisitor(this), t.Right.AcceptVisitor(this));
        }

        public IPartialType VisitSynonym(Types.TypeSynonym t)
        {
            return new TypeSynonym(t.Identifier, t.TypeParameters.Select(p => p.AcceptVisitor(this)).ToArray());
        }

        public IPartialType VisitProduct(Types.ProductType t)
        {
            return new ProductType(t.Left.AcceptVisitor(this), t.Right.AcceptVisitor(this));
        }

        public IPartialType VisitSum(Types.SumType t)
        {
            return new SumType(t.Left.AcceptVisitor(this), t.Right.AcceptVisitor(this));
        }

        public IPartialType VisitUnknown(Types.UnknownType t)
        {
            return solutionSet[t.Index];
        }

        public IPartialType VisitParameter(Types.TypeParameter t)
        {
            return new TypeParameter(t.Identifier);
        }
    }
}
