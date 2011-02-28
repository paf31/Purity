using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler
{
    public class DataSynonymRemover : IDataVisitor
    {
        public IData Result { get; set; }

        void IDataVisitor.VisitAna(Data.Ana d)
        {
            var visitor = new DataSynonymRemover();
            d.Coalgebra.AcceptVisitor(visitor);
            Result = new Data.Ana(visitor.Result);
        }

        void IDataVisitor.VisitApplication(Data.Application d)
        {
            var leftVisitor = new DataSynonymRemover();
            d.Left.AcceptVisitor(leftVisitor);
            var rightVisitor = new DataSynonymRemover();
            d.Right.AcceptVisitor(rightVisitor);
            Result = new Data.Application(leftVisitor.Result, rightVisitor.Result);
        }

        void IDataVisitor.VisitCase(Data.Case d)
        {
            var leftVisitor = new DataSynonymRemover();
            d.Left.AcceptVisitor(leftVisitor);
            var rightVisitor = new DataSynonymRemover();
            d.Right.AcceptVisitor(rightVisitor);
            Result = new Data.Case(leftVisitor.Result, rightVisitor.Result);
        }

        void IDataVisitor.VisitCata(Data.Cata d)
        {
            var visitor = new DataSynonymRemover();
            d.Algebra.AcceptVisitor(visitor);
            Result = new Data.Cata(visitor.Result);
        }

        void IDataVisitor.VisitComposition(Data.Composition d)
        {
            var leftVisitor = new DataSynonymRemover();
            d.Left.AcceptVisitor(leftVisitor);
            var rightVisitor = new DataSynonymRemover();
            d.Right.AcceptVisitor(rightVisitor);
            Result = new Data.Composition(leftVisitor.Result, rightVisitor.Result);
        }

        void IDataVisitor.VisitConst(Data.Const d)
        {
            var visitor = new DataSynonymRemover();
            d.Value.AcceptVisitor(visitor);
            Result = new Data.Const(visitor.Result);
        }

        void IDataVisitor.VisitSynonym(Data.DataSynonym d)
        {
            var target = Container.ResolveValue(d.Identifier);
            var visitor = new DataSynonymRemover();
            target.Data.AcceptVisitor(visitor);
            Result = visitor.Result;
            Result.Type = target.Type;
        }

        void IDataVisitor.VisitIdentity(Data.Identity d)
        {
            Result = d;
        }

        void IDataVisitor.VisitInl(Data.Inl d)
        {
            Result = d;
        }

        void IDataVisitor.VisitInr(Data.Inr d)
        {
            Result = d;
        }

        void IDataVisitor.VisitOutl(Data.Outl d)
        {
            Result = d;
        }

        void IDataVisitor.VisitOutr(Data.Outr d)
        {
            Result = d;
        }

        void IDataVisitor.VisitIn(Data.In d)
        {
            Result = d;
        }

        void IDataVisitor.VisitOut(Data.Out d)
        {
            Result = d;
        }

        void IDataVisitor.VisitSplit(Data.Split d)
        {
            var leftVisitor = new DataSynonymRemover();
            d.Left.AcceptVisitor(leftVisitor);
            var rightVisitor = new DataSynonymRemover();
            d.Right.AcceptVisitor(rightVisitor);
            Result = new Data.Split(leftVisitor.Result, rightVisitor.Result);
        }

        void IDataVisitor.VisitCurry(Data.Curried d)
        {
            var visitor = new DataSynonymRemover();
            d.Function.AcceptVisitor(visitor);
            Result = new Data.Curried(visitor.Result);
        }

        void IDataVisitor.VisitUncurry(Data.Uncurried d)
        {
            var visitor = new DataSynonymRemover();
            d.Function.AcceptVisitor(visitor);
            Result = new Data.Uncurried(visitor.Result);
        }

        void IDataVisitor.VisitCl(Data.Cl d)
        {
            var visitor = new DataSynonymRemover();
            d.Function.AcceptVisitor(visitor);
            Result = new Data.Cl(visitor.Result);
        }

        void IDataVisitor.VisitCr(Data.Cr d)
        {
            var visitor = new DataSynonymRemover();
            d.Function.AcceptVisitor(visitor);
            Result = new Data.Cr(visitor.Result);
        }
    }
}
