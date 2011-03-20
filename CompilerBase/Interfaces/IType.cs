using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Interfaces
{
    public interface IType
    {
        void AcceptVisitor(ITypeVisitor visitor);

        R AcceptVisitor<R>(ITypeVisitor<R> visitor);
    }
}
