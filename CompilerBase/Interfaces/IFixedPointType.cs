using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Interfaces
{
    public interface IFixedPointType : IType
    {
        string Identifier { get; set; }

        IFunctor Functor { get; set; }
    }
}
