using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Cata : ITypedExpression
    {
        public ITypedExpression Algebra
        {
            get;
            set;
        }

        public IType CarrierType
        {
            get;
            set;
        }

        public IType LFixType
        {
            get;
            set;
        }

        public Cata(ITypedExpression algebra, IType carrierType, IType lfixType)
        {
            Algebra = algebra;
            CarrierType = carrierType;
            LFixType = lfixType;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitCata(this);
        }
    }
}
