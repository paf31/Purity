using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Typechecker.Interfaces
{
    public interface IConstrainedData
    {
        void AcceptVisitor(IConstrainedDataVisitor visitor);

        R AcceptVisitor<R>(IConstrainedDataVisitor<R> visitor);
    }
}
