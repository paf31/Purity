using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.TypedExpressions;

namespace Purity.Compiler.Interfaces
{
    public interface ITypedExpressionVisitor
    {
        void VisitAna(Ana d);

        void VisitApplication(Application d);

        void VisitCase(Case d);

        void VisitCata(Cata d);

        void VisitComposition(Composition d);

        void VisitConst(Const d);

        void VisitIdentity(Identity d);

        void VisitInl(Inl d);

        void VisitInr(Inr d);

        void VisitOutl(Outl d);

        void VisitOutr(Outr d);

        void VisitSplit(Split d);

        void VisitIn(In d);

        void VisitOut(Out d);

        void VisitUncurry(Uncurried d);

        void VisitCurry(Curried d);

        void VisitSynonym(DataSynonym d);

        void VisitBox(Box d);

        void VisitUnbox(Unbox d);

        void VisitAbstraction(Abstraction d);

        void VisitVariable(Variable d);
    }

    public interface ITypedExpressionVisitor<R>
    {
        R VisitAna(Ana d);

        R VisitApplication(Application d);

        R VisitCase(Case d);

        R VisitCata(Cata d);

        R VisitComposition(Composition d);

        R VisitConst(Const d);

        R VisitIdentity(Identity d);

        R VisitInl(Inl d);

        R VisitInr(Inr d);

        R VisitOutl(Outl d);

        R VisitOutr(Outr d);

        R VisitSplit(Split d);

        R VisitIn(In d);

        R VisitOut(Out d);

        R VisitUncurry(Uncurried d);

        R VisitCurry(Curried d);

        R VisitSynonym(DataSynonym d);

        R VisitBox(Box d);

        R VisitUnbox(Unbox d);

        R VisitAbstraction(Abstraction d);

        R VisitVariable(Variable d);
    }
}
