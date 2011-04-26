using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class OccursCheck : IPartialTypeVisitor
    {
        private readonly int index;

        public static void ThrowOnOccurs(int index, IPartialType type)
        {
            type.AcceptVisitor(new OccursCheck(index));
        }

        public OccursCheck(int index)
        {
            this.index = index;
        }

        public void VisitArrow(Types.ArrowType t)
        {
            t.Left.AcceptVisitor(this);
            t.Right.AcceptVisitor(this);
        }

        public void VisitSynonym(Types.TypeSynonym t)
        {
            foreach (var t1 in t.TypeParameters)
            {
                t1.AcceptVisitor(this);
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

        public void VisitUnknown(Types.UnknownType t)
        {
            if (t.Index == index)
            {
                throw new CompilerException(ErrorMessages.CycleDetected);
            }
        }

        public void VisitParameter(Types.TypeParameter t)
        {
        }
    }
}
