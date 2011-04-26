using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Typechecker.Types;
using Purity.Compiler.Typechecker.Utilities;
using Purity.Compiler.Typechecker.Classes;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class TypeReplacer : IPartialTypeVisitor<IPartialType>
    {
        private readonly Constraint constraint;

        public static IPartialType Replace(IPartialType type, Constraint constraint)
        {
            var visitor = new TypeReplacer(constraint);
            return type.AcceptVisitor(visitor);
        }

        public TypeReplacer(Constraint constraint)
        {
            this.constraint = constraint;
        }

        public IPartialType VisitArrow(Types.ArrowType t)
        {
            return new ArrowType(t.Left.AcceptVisitor(this), t.Right.AcceptVisitor(this));
        }

        public IPartialType VisitSynonym(TypeSynonym t)
        {
            return new TypeSynonym(t.Identifier, t.TypeParameters.Select(p => p.AcceptVisitor(this)).ToArray());
        }

        public IPartialType VisitProduct(ProductType t)
        {
            return new ProductType(t.Left.AcceptVisitor(this), t.Right.AcceptVisitor(this));
        }

        public IPartialType VisitSum(SumType t)
        {
            return new SumType(t.Left.AcceptVisitor(this), t.Right.AcceptVisitor(this));
        }

        public IPartialType VisitUnknown(UnknownType t)
        {
            if (t.Index == constraint.Index)
            {
                return constraint.Type;
            }
            else
            {
                return t;
            }
        }

        public IPartialType VisitParameter(TypeParameter t)
        {
            return t;
        }
    }
}
