using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Data
{
    public class Abstraction : IConstrainedData
    {
        public string Variable
        {
            get;
            set;
        }

        public IConstrainedData Body
        {
            get;
            set;
        }

        public IPartialType VariableType
        {
            get;
            set;
        }

        public IPartialType BodyType
        {
            get;
            set;
        }

        public Abstraction(string variable, IConstrainedData body)
        {
            Variable = variable;
            Body = body;
        }

        public void AcceptVisitor(IConstrainedDataVisitor visitor)
        {
            visitor.VisitAbstraction(this);
        }

        public R AcceptVisitor<R>(IConstrainedDataVisitor<R> visitor)
        {
            return visitor.VisitAbstraction(this);
        }
    }
}
