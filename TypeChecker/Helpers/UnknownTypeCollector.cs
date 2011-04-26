using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Types;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class UnknownTypeCollector : IPartialTypeVisitor
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

        public static IEnumerable<int> Collect(IPartialType type)
        {
            var collector = new UnknownTypeCollector();
            type.AcceptVisitor(collector);
            return collector.Unknowns;
        }

        public void VisitArrow(ArrowType t)
        {
            t.Left.AcceptVisitor(this);
            t.Right.AcceptVisitor(this);
        }

        public void VisitSynonym(TypeSynonym t)
        {
            foreach (var typeParameter in t.TypeParameters) 
            {
                typeParameter.AcceptVisitor(this);
            }
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
    }
}
