using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class Curried : IConstrainedData
    {
        public IConstrainedData Function
        {
            get;
            set;
        }

        public Curried(IConstrainedData function, IPartialType first, IPartialType second, IPartialType output)
        {
            Function = function;
            First = first;
            Second = second;
            Output = output;
        }

        public IPartialType First
        {
            get;
            set;
        }

        public IPartialType Second
        {
            get;
            set;
        }

        public IPartialType Output
        {
            get;
            set;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitCurry(this);
        }

        public R AcceptVisitor<R>(IConstrainedDataVisitor<R> visitor)
        {
            return visitor.VisitCurry(this);
        }
    }
}
