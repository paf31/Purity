using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Typechecker.Types;
using Purity.Compiler.Typechecker.Functors;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class TableauMerger : IConstrainedDataVisitor
    {
        private readonly Tableau tableau;

        public IConstrainedData Result
        {
            get;
            set;
        }

        public IConstrainedData Merge(IConstrainedData data)
        {
            data.AcceptVisitor(this);
            return Result;
        }

        public TableauMerger(Tableau tableau)
        {
            this.tableau = tableau;
        }

        public void VisitAna(Data.Ana d)
        {
            var result = new Data.Ana(Merge(d.Coalgebra));
            result.CarrierType = tableau.Types[(d.CarrierType as UnknownType).Index];
            result.Functor = tableau.Functors[(d.Functor as UnknownFunctor).Index];
            result.GFixType = tableau.Types[(d.GFixType as UnknownType).Index];
            Result = result;
        }

        public void VisitApplication(Data.Application d)
        {
            var result = new Data.Application(Merge(d.Left), Merge(d.Right));
            result.LeftType = tableau.Types[(d.LeftType as UnknownType).Index];
            result.RightType = tableau.Types[(d.RightType as UnknownType).Index];
            Result = result;
        }

        public void VisitCase(Data.Case d)
        {
            var result = new Data.Case(Merge(d.Left), Merge(d.Right));
            result.LeftType = tableau.Types[(d.LeftType as UnknownType).Index];
            result.RightType = tableau.Types[(d.RightType as UnknownType).Index];
            result.ResultType = tableau.Types[(d.ResultType as UnknownType).Index];
            Result = result;
        }

        public void VisitCata(Data.Cata d)
        {
            var result = new Data.Cata(Merge(d.Algebra));
            result.CarrierType = tableau.Types[(d.CarrierType as UnknownType).Index];
            result.Functor = tableau.Functors[(d.Functor as UnknownFunctor).Index];
            result.LFixType = tableau.Types[(d.LFixType as UnknownType).Index];
            Result = result;
        }

        public void VisitComposition(Data.Composition d)
        {
            var result = new Data.Composition(Merge(d.Left), Merge(d.Right));
            result.LeftType = tableau.Types[(d.LeftType as UnknownType).Index];
            result.MiddleType = tableau.Types[(d.MiddleType as UnknownType).Index];
            result.RightType = tableau.Types[(d.RightType as UnknownType).Index];
            Result = result;
        }

        public void VisitConst(Data.Const d)
        {
            var result = new Data.Const(Merge(d.Value));
            result.InputType = tableau.Types[(d.InputType as UnknownType).Index];
            result.OutputType = tableau.Types[(d.OutputType as UnknownType).Index];
            Result = result;
        }

        public void VisitIdentity(Data.Identity d)
        {
            var result = new Data.Identity();
            result.Type = tableau.Types[(d.Type as UnknownType).Index];
            Result = result;
        }

        public void VisitInl(Data.Inl d)
        {
            var result = new Data.Inl();
            result.LeftType = tableau.Types[(d.LeftType as UnknownType).Index];
            result.RightType = tableau.Types[(d.RightType as UnknownType).Index];
            Result = result;
        }

        public void VisitInr(Data.Inr d)
        {
            var result = new Data.Inr();
            result.LeftType = tableau.Types[(d.LeftType as UnknownType).Index];
            result.RightType = tableau.Types[(d.RightType as UnknownType).Index];
            Result = result;
        }

        public void VisitOutl(Data.Outl d)
        {
            var result = new Data.Outl();
            result.LeftType = tableau.Types[(d.LeftType as UnknownType).Index];
            result.RightType = tableau.Types[(d.RightType as UnknownType).Index];
            Result = result;
        }

        public void VisitOutr(Data.Outr d)
        {
            var result = new Data.Outr();
            result.LeftType = tableau.Types[(d.LeftType as UnknownType).Index];
            result.RightType = tableau.Types[(d.RightType as UnknownType).Index];
            Result = result;
        }

        public void VisitSplit(Data.Split d)
        {
            var result = new Data.Split(Merge(d.Left), Merge(d.Right));
            result.LeftType = tableau.Types[(d.LeftType as UnknownType).Index];
            result.RightType = tableau.Types[(d.RightType as UnknownType).Index];
            result.InputType = tableau.Types[(d.InputType as UnknownType).Index];
            Result = result;
        }

        public void VisitIn(Data.In d)
        {
            var result = new Data.In();
            result.Source = tableau.Types[(d.Source as UnknownType).Index];
            result.Functor = tableau.Functors[(d.Functor as UnknownFunctor).Index];
            Result = result;
        }

        public void VisitOut(Data.Out d)
        {
            var result = new Data.Out();
            result.Target = tableau.Types[(d.Target as UnknownType).Index];
            result.Functor = tableau.Functors[(d.Functor as UnknownFunctor).Index];
            Result = result;
        }

        public void VisitCurry(Data.Curried d)
        {
            var result = new Data.Curried(Merge(d.Function));
            result.First = tableau.Types[(d.First as UnknownType).Index];
            result.Second = tableau.Types[(d.Second as UnknownType).Index];
            result.Output = tableau.Types[(d.Output as UnknownType).Index];
            Result = result;
        }

        public void VisitUncurry(Data.Uncurried d)
        {
            var result = new Data.Uncurried(Merge(d.Function));
            result.First = tableau.Types[(d.First as UnknownType).Index];
            result.Second = tableau.Types[(d.Second as UnknownType).Index];
            result.Output = tableau.Types[(d.Output as UnknownType).Index];
            Result = result;
        }

        public void VisitCl(Data.Cl d)
        {
            var result = new Data.Cl(Merge(d.Function));
            result.A = tableau.Types[(d.A as UnknownType).Index];
            result.B = tableau.Types[(d.B as UnknownType).Index];
            result.C = tableau.Types[(d.C as UnknownType).Index];
            Result = result;
        }

        public void VisitCr(Data.Cr d)
        {
            var result = new Data.Cr(Merge(d.Function));
            result.A = tableau.Types[(d.A as UnknownType).Index];
            result.B = tableau.Types[(d.B as UnknownType).Index];
            result.C = tableau.Types[(d.C as UnknownType).Index];
            Result = result;
        }

        public void VisitSynonym(Data.DataSynonym d)
        {
            var result = new Data.DataSynonym(d.Identifier);
            Result = result;
        }

        public void VisitBox(Data.Box d)
        {
            var result = new Data.Box();
            result.Target = tableau.Types[(d.Target as UnknownType).Index];
            result.Type = tableau.Types[(d.Type as UnknownType).Index];
            Result = result;
        }

        public void VisitUnbox(Data.Unbox d)
        {
            var result = new Data.Unbox();
            result.Target = tableau.Types[(d.Target as UnknownType).Index];
            result.Type = tableau.Types[(d.Type as UnknownType).Index];
            Result = result;
        }
    }
}
