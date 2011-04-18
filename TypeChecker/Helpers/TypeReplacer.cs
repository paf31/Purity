using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Typechecker.Types;
using Purity.Compiler.Typechecker.Utilities;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class TypeReplacer : IPartialTypeVisitor
    {
        private readonly int index;
        private readonly Tableau tableau;
        private readonly IPartialType replacement;

        public bool HasChanges
        {
            get;
            set;
        }

        public static bool Replace(IPartialType type, Tableau tableau, int index, IPartialType replacement)
        {
            var visitor = new TypeReplacer(index, tableau, replacement);
            type.AcceptVisitor(visitor);
            return visitor.HasChanges;
        }

        public TypeReplacer(int index, Tableau tableau, IPartialType replacement)
        {
            this.index = index;
            this.tableau = tableau;
            this.replacement = replacement;
            this.HasChanges = false;
        }

        public void VisitArrow(Types.ArrowType t)
        {
            if (t.Left is UnknownType && (t.Left as UnknownType).Index == index)
            {
                var unknown = replacement as UnknownType;
                HasChanges |= !(unknown != null && unknown.Index == index);
                t.Left = replacement;
            }

            if (t.Right is UnknownType && (t.Right as UnknownType).Index == index)
            {
                var unknown = replacement as UnknownType;
                HasChanges |= !(unknown != null && unknown.Index == index);
                t.Right = replacement;
            }

            t.Left.AcceptVisitor(this);
            t.Right.AcceptVisitor(this);
        }

        public void VisitSynonym(Types.TypeSynonym t)
        {
            for (int i = 0; i< t.TypeParameters.Length; i++)
            {
                if (t.TypeParameters[i] is UnknownType && (t.TypeParameters[i] as UnknownType).Index == index)
                {
                    var unknown = replacement as UnknownType;
                    HasChanges |= !(unknown != null && unknown.Index == index);
                    t.TypeParameters[i] = replacement;
                }

                t.TypeParameters[i].AcceptVisitor(this);
            }
        }

        public void VisitProduct(Types.ProductType t)
        {
            if (t.Left is UnknownType && (t.Left as UnknownType).Index == index)
            {
                var unknown = replacement as UnknownType;
                HasChanges |= !(unknown != null && unknown.Index == index);
                t.Left = replacement;
            }

            if (t.Right is UnknownType && (t.Right as UnknownType).Index == index)
            {
                var unknown = replacement as UnknownType;
                HasChanges |= !(unknown != null && unknown.Index == index);
                t.Right = replacement;
            }

            t.Left.AcceptVisitor(this);
            t.Right.AcceptVisitor(this);
        }

        public void VisitSum(Types.SumType t)
        {
            if (t.Left is UnknownType && (t.Left as UnknownType).Index == index)
            {
                var unknown = replacement as UnknownType;
                HasChanges |= !(unknown != null && unknown.Index == index);
                t.Left = replacement;
            }

            if (t.Right is UnknownType && (t.Right as UnknownType).Index == index)
            {
                var unknown = replacement as UnknownType;
                HasChanges |= !(unknown != null && unknown.Index == index);
                t.Right = replacement;
            }

            t.Left.AcceptVisitor(this);
            t.Right.AcceptVisitor(this);
        }

        public void VisitUnknown(Types.UnknownType t)
        {
        }

        public void VisitParameter(TypeParameter t)
        {
        }
    }
}
