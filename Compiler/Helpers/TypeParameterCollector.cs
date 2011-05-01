using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Helpers
{
    public class TypeParameterCollector : ITypeVisitor
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
            foreach (var typeParameter in t.TypeParameters)
            {
                typeParameter.AcceptVisitor(this);
            }
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

        public void VisitParameter(Types.TypeParameter t)
        {
            if (!Parameters.Contains(t.Identifier))
            {
                Parameters.Add(t.Identifier);
            }
        }
    }
}
