using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Typechecker.Interfaces
{
    public interface IPartialType
    {
        void AcceptVisitor(IPartialTypeVisitor visitor);

        R AcceptVisitor<R>(IPartialTypeVisitor<R> visitor);
    }
}
