using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Types;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class UnknownTypeCollector : IPartialTypeVisitor, IPartialFunctorVisitor
    {
        public IList<int> Unknowns
        {
            get;
            set;
        }

        public UnknownTypeCollector()
        {
            Unknowns = new List<int>();
        }

        public void VisitArrow(ArrowType t)
        {
            t.Left.AcceptVisitor(this);
            t.Right.AcceptVisitor(this);
        }

        public void VisitSynonym(TypeSynonym t)
        {
        }

        public void VisitProduct(ProductType t)
        {
            t.Left.AcceptVisitor(this);
            t.Right.AcceptVisitor(this);
        }

        public void VisitSum(SumType t)
        {
            t.Left.AcceptVisitor(this);
            t.Right.AcceptVisitor(this);
        }

        public void VisitLFix(LFixType t)
        {
            t.Functor.AcceptVisitor(this);
        }

        public void VisitGFix(GFixType t)
        {
            t.Functor.AcceptVisitor(this);
        }

        public void VisitUnknown(UnknownType t)
        {
            if (!Unknowns.Contains(t.Index))
            {
                Unknowns.Add(t.Index);
            }
        }

        public void VisitParameter(TypeParameter t)
        {
        }

        public void VisitArrow(Functors.ArrowFunctor f)
        {
            f.Left.AcceptVisitor(this);
            f.Right.AcceptVisitor(this);
        }

        public void VisitConstant(Functors.ConstantFunctor f)
        {
            f.Value.AcceptVisitor(this);
        }

        public void VisitIdentity(Functors.IdentityFunctor f)
        {
        }

        public void VisitProduct(Functors.ProductFunctor f)
        {
            f.Left.AcceptVisitor(this);
            f.Right.AcceptVisitor(this);
        }

        public void VisitSum(Functors.SumFunctor f)
        {
            f.Left.AcceptVisitor(this);
            f.Right.AcceptVisitor(this);
        }

        public void VisitUnknown(Functors.UnknownFunctor f)
        {
        }

        public void VisitSynonym(Functors.FunctorSynonym f)
        {
        }
    }
}
