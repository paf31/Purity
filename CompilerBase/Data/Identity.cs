using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Data
{
    public class Identity : IData
    {
        public void AcceptVisitor(IDataVisitor visitor)
        {
            visitor.VisitIdentity(this);
        }
    }
}
