using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Data
{
    public class Case : IData
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

        public Case(IData left, IData right)
        {
            Left = left;
            Right = right;
        }

        public void AcceptVisitor(IDataVisitor visitor)
        {
            visitor.VisitCase(this);
        }

        public R AcceptVisitor<R>(IDataVisitor<R> visitor)
        {
            return visitor.VisitCase(this);
        }
    }
}
