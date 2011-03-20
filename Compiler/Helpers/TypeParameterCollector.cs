using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Helpers
{
    public class TypeParameterCollector : ITypeVisitor, IFunctorVisitor
    {
        public IList<string> Parameters
        {
            get;
            set;
        }

        public TypeParameterCollector()
        {
            Parameters = new List<string>();
        }

        public void VisitArrow(Types.ArrowType t)
        {
            t.Left.AcceptVisitor(this);
            t.Right.AcceptVisitor(this);
        }

        public void VisitSynonym(Types.TypeSynonym t)
        {
        }

        public void VisitProduct(Types.ProductType t)
        {
            t.Left.AcceptVisitor(this);
            t.Right.AcceptVisitor(this);
        }

        public void VisitSum(Types.SumType t)
        {
            t.Left.AcceptVisitor(this);
            t.Right.AcceptVisitor(this);
        }

        public void VisitLFix(Types.LFixType t)
        {
            t.Functor.AcceptVisitor(this);
        }

        public void VisitGFix(Types.GFixType t)
        {
            t.Functor.AcceptVisitor(this);
        }

        public void VisitParameter(Types.TypeParameter t)
        {
            if (!Parameters.Contains(t.Identifier))
            {
                Parameters.Add(t.Identifier);
            }
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

        public void VisitSynonym(Functors.FunctorSynonym f)
        {
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
    }
}
