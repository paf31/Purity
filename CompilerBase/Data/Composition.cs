using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Data
{
    public class Composition : IData
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

        public Composition(IData left, IData right)
        {
            Left = left;
            Right = right;
        }

        public void AcceptVisitor(IDataVisitor visitor)
        {
            visitor.VisitComposition(this);
        }

        public R AcceptVisitor<R>(IDataVisitor<R> visitor)
        {
            return visitor.VisitComposition(this);
        }
    }
}
