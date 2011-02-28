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

        void VisitUncurry(Uncurried uncurried);

        void VisitCurry(Curried curried);

        void VisitCl(Cl cl);

        void VisitCr(Cr cr);
    }       
}
