using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Repl.Helpers
{
    public class PrintType : ITypeVisitor<string>
    {
        public static string Print(IType t)
        {
            return t.AcceptVisitor(new PrintType());
        }

        public string VisitArrow(Purity.Compiler.Types.ArrowType t)
        {
            return string.Format("({0} -> {1})", t.Left.AcceptVisitor(this), t.Right.AcceptVisitor(this));
        }

        public string VisitSynonym(Purity.Compiler.Types.TypeSynonym t)
        {
            string result = t.Identifier;

            if (t.TypeParameters.Any())
            {
                result += ' ';
                result += string.Join(" ", t.TypeParameters.Select(p => p.AcceptVisitor(this)).ToArray());
            }

            return result;
        }

        public string VisitProduct(Purity.Compiler.Types.ProductType t)
        {
            return string.Format("({0} . {1})", t.Left.AcceptVisitor(this), t.Right.AcceptVisitor(this));
        }

        public string VisitSum(Purity.Compiler.Types.SumType t)
        {
            return string.Format("({0} + {1})", t.Left.AcceptVisitor(this), t.Right.AcceptVisitor(this));
        }

        public string VisitParameter(Purity.Compiler.Types.TypeParameter t)
        {
            return string.Format("{0}?", t.Identifier);
        }
    }
}
