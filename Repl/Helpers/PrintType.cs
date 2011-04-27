using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Repl.Helpers
{
    public class PrintType : ITypeVisitor<string>, IFunctorVisitor<string>
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
            return string.Format("{0} {1}", t.Identifier, string.Join(" ", t.TypeParameters.Select(p => p.AcceptVisitor(this)).ToArray()));
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

        public string VisitArrow(Purity.Compiler.Functors.ArrowFunctor f)
        {
            return string.Format("({0} -> {1})", f.Left.AcceptVisitor(this), f.Right.AcceptVisitor(this));
        }

        public string VisitConstant(Purity.Compiler.Functors.ConstantFunctor f)
        {
            return string.Format("const {0}", f.Value.AcceptVisitor(this));
        }

        public string VisitSynonym(Purity.Compiler.Functors.FunctorSynonym f)
        {
            return f.Identifier;
        }

        public string VisitIdentity(Purity.Compiler.Functors.IdentityFunctor f)
        {
            return "id";
        }

        public string VisitProduct(Purity.Compiler.Functors.ProductFunctor f)
        {
            return string.Format("({0} . {1})", f.Left.AcceptVisitor(this), f.Right.AcceptVisitor(this));
        }

        public string VisitSum(Purity.Compiler.Functors.SumFunctor f)
        {
            return string.Format("({0} + {1})", f.Left.AcceptVisitor(this), f.Right.AcceptVisitor(this));
        }
    }
}
