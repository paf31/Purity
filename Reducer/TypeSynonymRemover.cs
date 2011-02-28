using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler
{
    public class TypeSynonymRemover : ITypeVisitor
    {
        public IType Result { get; set; }

        void ITypeVisitor.VisitArrow(Types.ArrowType t)
        {
            var leftVisitor = new TypeSynonymRemover();
            t.Left.AcceptVisitor(leftVisitor);
            var rightVisitor = new TypeSynonymRemover();
            t.Right.AcceptVisitor(rightVisitor);
            Result = new Types.ArrowType(leftVisitor.Result, rightVisitor.Result);
        }

        void ITypeVisitor.VisitSynonym(Types.TypeSynonym t)
        {
            var target = Container.ResolveType(t.Identifier);
            var visitor = new TypeSynonymRemover();
            target.AcceptVisitor(visitor);
            Result = visitor.Result;
        }

        void ITypeVisitor.VisitProduct(Types.ProductType t)
        {
            var leftVisitor = new TypeSynonymRemover();
            t.Left.AcceptVisitor(leftVisitor);
            var rightVisitor = new TypeSynonymRemover();
            t.Right.AcceptVisitor(rightVisitor);
            Result = new Types.ProductType(leftVisitor.Result, rightVisitor.Result);
        }

        void ITypeVisitor.VisitSum(Types.SumType t)
        {
            var leftVisitor = new TypeSynonymRemover();
            t.Left.AcceptVisitor(leftVisitor);
            var rightVisitor = new TypeSynonymRemover();
            t.Right.AcceptVisitor(rightVisitor);
            Result = new Types.SumType(leftVisitor.Result, rightVisitor.Result);
        }

        void ITypeVisitor.VisitLFix(Types.LFixType t)
        {
            var visitor = new FunctorSynonymRemover();
            t.Functor.AcceptVisitor(visitor);
            Result = new Types.LFixType(visitor.Result);
        }

        void ITypeVisitor.VisitGFix(Types.GFixType t)
        {
            var visitor = new FunctorSynonymRemover();
            t.Functor.AcceptVisitor(visitor);
            Result = new Types.GFixType(visitor.Result);
        }

        void ITypeVisitor.VisitFunctorApp(Types.FunctorAppType t)
        {
            var functorVisitor = new FunctorSynonymRemover();
            t.Functor.AcceptVisitor(functorVisitor);
            var argVisitor = new TypeSynonymRemover();
            t.Argument.AcceptVisitor(argVisitor);
            Result = new Types.FunctorAppType(functorVisitor.Result, argVisitor.Result);
        }
    }
}
