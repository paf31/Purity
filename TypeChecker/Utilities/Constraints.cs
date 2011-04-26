using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Classes;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Utilities
{
    public static class Constraints
    {
        public static IEnumerable<Constraint> Identity()
        {
            return Enumerable.Empty<Constraint>();
        }

        public static IEnumerable<Constraint> Single(int index, IPartialType type)
        {
            yield return new Constraint(index, type);
        }
    }
}
