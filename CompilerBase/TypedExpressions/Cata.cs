using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypedExpressions
{
    public class Cata : ITypedExpression
    {
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

        public Cata(IType carrierType, IType lfixType)
        {
            CarrierType = carrierType;
            LFixType = lfixType;
        }

        public void AcceptVisitor(ITypedExpressionVisitor visitor)
        {
            visitor.VisitCata(this);
        }
    }
}
