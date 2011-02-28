using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Data
{
    public class Split : IData
    {
        public IData Left
        {
            get;
            set;
        }

        public IData Right
        {
            get;
            set;
        }

        public Split(IData left, IData right)
        {
            Left = left;
            Right = right;
        }

        public void AcceptVisitor(IDataVisitor visitor)
        {
            visitor.VisitSplit(this);
        }

        public IType Type
        {
            get;
            set;
        }
    }
}
