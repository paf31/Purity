using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class Uncurried : IConstrainedData
    {
        public IConstrainedData Function
        {
            get;
            set;
        }

        public Uncurried(IConstrainedData function, IPartialType first, IPartialType second, IPartialType output)
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
            visitor.VisitUncurry(this);
        }

        public R AcceptVisitor<R>(IConstrainedDataVisitor<R> visitor)
        {
            return visitor.VisitUncurry(this);
        }
    }
}
