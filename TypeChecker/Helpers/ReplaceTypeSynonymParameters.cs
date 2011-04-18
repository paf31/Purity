using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class ReplaceTypeSynonymParameters : IPartialTypeVisitor<IPartialType>
    {
        private readonly IDictionary<string, IPartialType> types;

        public ReplaceTypeSynonymParameters(IDictionary<string, IPartialType> types)
        {
            this.types = types;
        }

        public static IPartialType Replace(IPartialType type, string[] names, IPartialType[] types)
        {
            var dictionary = names.Zip(types, (s, t) => Tuple.Create(s, t)).ToDictionary(t => t.Item1, t => t.Item2);
            return Replace(type, dictionary);
        }

        public static IPartialType Replace(IPartialType type, IDictionary<string, IPartialType> types)
        {
            return type.AcceptVisitor(new ReplaceTypeSynonymParameters(types));
        }

        public IPartialType VisitArrow(Types.ArrowType t)
        {
            return new Types.ArrowType(t.Left.AcceptVisitor(this), t.Right.AcceptVisitor(this));
        }

        public IPartialType VisitSynonym(Types.TypeSynonym t)
        {
            return new Types.TypeSynonym(t.Identifier, t.TypeParameters.Select(t1 => t1.AcceptVisitor(this)).ToArray());
        }

        public IPartialType VisitProduct(Types.ProductType t)
        {
            return new Types.ProductType(t.Left.AcceptVisitor(this), t.Right.AcceptVisitor(this));
        }

        public IPartialType VisitSum(Types.SumType t)
        {
            return new Types.SumType(t.Left.AcceptVisitor(this), t.Right.AcceptVisitor(this));
        }

        public IPartialType VisitUnknown(Types.UnknownType t)
        {
            return t;
        }

        public IPartialType VisitParameter(Types.TypeParameter t)
        {
            return types[t.Identifier];
        }
    }
}
