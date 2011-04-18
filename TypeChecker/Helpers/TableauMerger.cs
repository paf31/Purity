using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Typechecker.Types;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class TableauMerger : IConstrainedDataVisitor<IConstrainedData>
    {
        private readonly Tableau tableau;

        public TableauMerger(Tableau tableau)
        {
            this.tableau = tableau;
        }

        public static IConstrainedData Merge(IConstrainedData data, Tableau tableau)
        {
            return data.AcceptVisitor(new TableauMerger(tableau));
        }

        public IConstrainedData VisitApplication(Data.Application d)
        {
            var result = new Data.Application(Merge(d.Left, tableau), Merge(d.Right, tableau));
            result.LeftType = tableau.Types[(d.LeftType as UnknownType).Index];
            result.RightType = tableau.Types[(d.RightType as UnknownType).Index];
            return result;
        }

        public IConstrainedData VisitCase(Data.Case d)
        {
            var result = new Data.Case(Merge(d.Left, tableau), Merge(d.Right, tableau));
            result.LeftType = tableau.Types[(d.LeftType as UnknownType).Index];
            result.RightType = tableau.Types[(d.RightType as UnknownType).Index];
            result.ResultType = tableau.Types[(d.ResultType as UnknownType).Index];
            return result;
        }

        public IConstrainedData VisitComposition(Data.Composition d)
        {
            var result = new Data.Composition(Merge(d.Left, tableau), Merge(d.Right, tableau));
            result.LeftType = tableau.Types[(d.LeftType as UnknownType).Index];
            result.MiddleType = tableau.Types[(d.MiddleType as UnknownType).Index];
            result.RightType = tableau.Types[(d.RightType as UnknownType).Index];
            return result;
        }

        public IConstrainedData VisitConst(Data.Const d)
        {
            var result = new Data.Const(Merge(d.Value, tableau));
            result.InputType = tableau.Types[(d.InputType as UnknownType).Index];
            result.OutputType = tableau.Types[(d.OutputType as UnknownType).Index];
            return result;
        }

        public IConstrainedData VisitIdentity(Data.Identity d)
        {
            var result = new Data.Identity();
            result.Type = tableau.Types[(d.Type as UnknownType).Index];
            return result;
        }

        public IConstrainedData VisitInl(Data.Inl d)
        {
            var result = new Data.Inl();
            result.LeftType = tableau.Types[(d.LeftType as UnknownType).Index];
            result.RightType = tableau.Types[(d.RightType as UnknownType).Index];
            return result;
        }

        public IConstrainedData VisitInr(Data.Inr d)
        {
            var result = new Data.Inr();
            result.LeftType = tableau.Types[(d.LeftType as UnknownType).Index];
            result.RightType = tableau.Types[(d.RightType as UnknownType).Index];
            return result;
        }

        public IConstrainedData VisitOutl(Data.Outl d)
        {
            var result = new Data.Outl();
            result.LeftType = tableau.Types[(d.LeftType as UnknownType).Index];
            result.RightType = tableau.Types[(d.RightType as UnknownType).Index];
            return result;
        }

        public IConstrainedData VisitOutr(Data.Outr d)
        {
            var result = new Data.Outr();
            result.LeftType = tableau.Types[(d.LeftType as UnknownType).Index];
            result.RightType = tableau.Types[(d.RightType as UnknownType).Index];
            return result;
        }

        public IConstrainedData VisitSplit(Data.Split d)
        {
            var result = new Data.Split(Merge(d.Left, tableau), Merge(d.Right, tableau));
            result.LeftType = tableau.Types[(d.LeftType as UnknownType).Index];
            result.RightType = tableau.Types[(d.RightType as UnknownType).Index];
            result.InputType = tableau.Types[(d.InputType as UnknownType).Index];
            return result;
        }

        public IConstrainedData VisitCurry(Data.Curried d)
        {
            var result = new Data.Curried(Merge(d.Function, tableau));
            result.First = tableau.Types[(d.First as UnknownType).Index];
            result.Second = tableau.Types[(d.Second as UnknownType).Index];
            result.Output = tableau.Types[(d.Output as UnknownType).Index];
            return result;
        }

        public IConstrainedData VisitUncurry(Data.Uncurried d)
        {
            var result = new Data.Uncurried(Merge(d.Function, tableau));
            result.First = tableau.Types[(d.First as UnknownType).Index];
            result.Second = tableau.Types[(d.Second as UnknownType).Index];
            result.Output = tableau.Types[(d.Output as UnknownType).Index];
            return result;
        }

        public IConstrainedData VisitSynonym(Data.DataSynonym d)
        {
            var result = new Data.DataSynonym(d.Identifier);
            
            foreach (var pair in d.TypeParameters)
            {
                result.TypeParameters[pair.Key] = tableau.Types[(pair.Value as UnknownType).Index];
            }

            return result;
        }

        public IConstrainedData VisitAbstraction(Data.Abstraction d)
        {
            var result = new Data.Abstraction(d.Variable, Merge(d.Body, tableau));
            result.VariableType = tableau.Types[(d.VariableType as UnknownType).Index];
            result.BodyType = tableau.Types[(d.BodyType as UnknownType).Index];
            return result;
        }

        public IConstrainedData VisitVariable(Data.Variable d)
        {
            var result = new Data.Variable(d.Name);
            result.Type = tableau.Types[(d.Type as UnknownType).Index];
            return result;
        }
    }
}
