using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Data;

namespace Purity.Compiler.Interfaces
{
    public interface IDataVisitor
    {
        void VisitAna(Ana d);

        void VisitApplication(Application d);

        void VisitCase(Case d);

        void VisitCata(Cata d);

        void VisitComposition(Composition d);

        void VisitConst(Const d);

        void VisitSynonym(DataSynonym d);

        void VisitIdentity(Identity d);

        void VisitInl(Inl d);

        void VisitInr(Inr d);

        void VisitOutl(Outl d);

        void VisitOutr(Outr d);

        void VisitSplit(Split d);

        void VisitIn(In d);

        void VisitOut(Out d);

        void VisitCurry(Curried d);

        void VisitUncurry(Uncurried d);

        void VisitCl(Cl d);

        void VisitCr(Cr d);

        void VisitBox(Box d);

        void VisitUnbox(Unbox d);
    }       
}
