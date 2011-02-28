using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;

namespace Purity.Compiler.Interfaces
{
    public interface IFunctor
    {
        void AcceptVisitor(IFunctorVisitor visitor);
    }
}
