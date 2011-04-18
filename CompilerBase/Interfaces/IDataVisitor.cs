using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Data;

namespace Purity.Compiler.Interfaces
{
    public interface IDataVisitor
    {
        void VisitApplication(Application d);

        void VisitCase(Case d);

        void VisitComposition(Composition d);

        void VisitConst(Const d);

        void VisitSynonym(DataSynonym d);

        void VisitIdentity(Identity d);

        void VisitInl(Inl d);

        void VisitInr(Inr d);

        void VisitOutl(Outl d);

        void VisitOutr(Outr d);

        void VisitSplit(Split d);

        void VisitCurry(Curried d);

        void VisitUncurry(Uncurried d);

        void VisitAbstraction(Abstraction d);

        void VisitVariable(Variable d);
    }

    public interface IDataVisitor<R>
    {
        R VisitApplication(Application d);

        R VisitCase(Case d);

        R VisitComposition(Composition d);

        R VisitConst(Const d);

        R VisitSynonym(DataSynonym d);

        R VisitIdentity(Identity d);

        R VisitInl(Inl d);

        R VisitInr(Inr d);

        R VisitOutl(Outl d);

        R VisitOutr(Outr d);

        R VisitSplit(Split d);

        R VisitCurry(Curried d);

        R VisitUncurry(Uncurried d);

        R VisitAbstraction(Abstraction d);

        R VisitVariable(Variable d);
    }       
}
