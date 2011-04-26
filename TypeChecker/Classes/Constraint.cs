using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Classes
{
    public class Constraint
    {
        public int Index
        {
            get;
            set;
        }

        public IPartialType Type
        {
            get;
            set;
        }

        public Constraint(int index, IPartialType type)
        {
            Index = index;
            Type = type;
        }
    }
}
