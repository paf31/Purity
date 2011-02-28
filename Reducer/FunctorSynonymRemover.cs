using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler
{
    public class FunctorSynonymRemover : IFunctorVisitor
    {
        public IFunctor Result { get; set; }

        void IFunctorVisitor.VisitArrow(Functors.ArrowFunctor f)
        {
            var leftVisitor = new TypeSynonymRemover();
            f.Left.AcceptVisitor(leftVisitor);
            var rightVisitor = new FunctorSynonymRemover();
            f.Right.AcceptVisitor(rightVisitor);
            Result = new Functors.ArrowFunctor(leftVisitor.Result, rightVisitor.Result);
        }

        void IFunctorVisitor.VisitConstant(Functors.ConstantFunctor f)
        {
            var valueVisitor = new TypeSynonymRemover();
            f.Value.AcceptVisitor(valueVisitor);
            Result = new Functors.ConstantFunctor(valueVisitor.Result);
        }

        void IFunctorVisitor.VisitSynonym(Functors.FunctorSynonym f)
        {
            var target = Container.ResolveFunctor(f.Identifier);
            var visitor = new FunctorSynonymRemover();
            target.AcceptVisitor(visitor);
            Result = visitor.Result;
        }

        void IFunctorVisitor.VisitIdentity(Functors.IdentityFunctor f)
        {
            Result = f;
        }

        void IFunctorVisitor.VisitProduct(Functors.ProductFunctor f)
        {
            var leftVisitor = new FunctorSynonymRemover();
            f.Left.AcceptVisitor(leftVisitor);
            var rightVisitor = new FunctorSynonymRemover();
            f.Right.AcceptVisitor(rightVisitor);
            Result = new Functors.ProductFunctor(leftVisitor.Result, rightVisitor.Result);
        }

        void IFunctorVisitor.VisitSum(Functors.SumFunctor f)
        {
            var leftVisitor = new FunctorSynonymRemover();
            f.Left.AcceptVisitor(leftVisitor);
            var rightVisitor = new FunctorSynonymRemover();
            f.Right.AcceptVisitor(rightVisitor);
            Result = new Functors.SumFunctor(leftVisitor.Result, rightVisitor.Result);
        }
    }
}
