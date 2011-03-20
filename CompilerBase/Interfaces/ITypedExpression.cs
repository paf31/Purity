using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Interfaces
{
    public interface ITypedExpression
    {
        void AcceptVisitor(ITypedExpressionVisitor visitor);

        R AcceptVisitor<R>(ITypedExpressionVisitor<R> visitor);
    }
}
