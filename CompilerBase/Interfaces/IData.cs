using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Interfaces
{
    public interface IData
    {
        void AcceptVisitor(IDataVisitor visitor);
    }
}
