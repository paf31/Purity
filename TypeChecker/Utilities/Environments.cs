using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Classes;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Utilities
{
    public static class Environments
    {
        public static TypeCheckingEnvironment Empty()
        {
            return ident => null;
        }

        public static TypeCheckingEnvironment Replace(this TypeCheckingEnvironment e, string ident, IPartialType type)
        {
            return ident1 => ident.Equals(ident1) ? type : e(ident1);
        }
    }
}
