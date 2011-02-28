using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class Composition : IConstrainedData
    {
        public IConstrainedData Left
        {
            get;
            set;
        }

        public IConstrainedData Right
        {
            get;
            set;
        }

        public IPartialType LeftType
        {
            get;
            set;
        }

        public IPartialType MiddleType
        {
            get;
            set;
        }

        public IPartialType RightType
        {
            get;
            set;
        }

        public Composition(IConstrainedData left, IConstrainedData right)
        {
            Left = left;
            Right = right;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitComposition(this);
        }
    }
}
