using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class PartialTypeCreator : ITypeVisitor<IPartialType>
    {
        private readonly Func<string, IPartialType> lookup;

        public PartialTypeCreator(Func<string, IPartialType> lookup)
        {
            this.lookup = lookup;
        }

        public static IPartialType Convert(IType type, Func<string, IPartialType> lookup)
        {
            var visitor = new PartialTypeCreator(lookup);
            return type.AcceptVisitor(visitor);
        }

        public IPartialType VisitArrow(Compiler.Types.ArrowType t)
        {
            return new Types.ArrowType(Convert(t.Left, lookup), Convert(t.Right, lookup));
        }

        public IPartialType VisitSynonym(Compiler.Types.TypeSynonym t)
        {
            return new Types.TypeSynonym(t.Identifier, t.TypeParameters.Select(p => Convert(p, lookup)).ToArray());
        }

        public IPartialType VisitProduct(Compiler.Types.ProductType t)
        {
            return new Types.ProductType(Convert(t.Left, lookup), Convert(t.Right, lookup));
        }

        public IPartialType VisitSum(Compiler.Types.SumType t)
        {
            return new Types.SumType(Convert(t.Left, lookup), Convert(t.Right, lookup));
        }

        public IPartialType VisitParameter(Compiler.Types.TypeParameter t)
        {
            return lookup == null ? new Types.TypeParameter(t.Identifier) : lookup(t.Identifier);
        }
    }
}
