using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Typechecker.Types;

namespace Purity.Compiler.Typechecker.Classes
{
    public class TypeCheckingContext
    {
        private int index;

        public int Index
        {
            get
            {
                return index;
            }
        }

        public IPartialType NewType()
        {
            return new UnknownType(index++);
        }
    }
}
