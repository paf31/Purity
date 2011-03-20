using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class Const : IConstrainedData
    {
        public IConstrainedData Value
        {
            get;
            set;
        }

        public IPartialType InputType
        {
            get;
            set;
        }

        public IPartialType OutputType
        {
            get;
            set;
        }

        public Const(IConstrainedData value)
        {
            Value = value;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitConst(this);
        }

        public R AcceptVisitor<R>(IConstrainedDataVisitor<R> visitor)
        {
            return visitor.VisitConst(this);
        }
    }
}
