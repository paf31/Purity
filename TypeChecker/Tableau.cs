using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Typechecker.Types;
using Purity.Compiler.Typechecker.Functors;

namespace Purity.Compiler.Typechecker
{
    public class Tableau
    {
        public IDictionary<int, IPartialType> Types
        {
            get;
            set;
        }

        public IDictionary<int, IPartialFunctor> Functors
        {
            get;
            set;
        }

        public Tableau()
        {
            Types = new Dictionary<int, IPartialType>();
            Functors = new Dictionary<int, IPartialFunctor>();
        }

        public Tableau(int types, int functors)
        {
            Types = new Dictionary<int, IPartialType>();
            Functors = new Dictionary<int, IPartialFunctor>();

            for (int i = 0; i < types; i++)
            {
                Types[i] = new UnknownType(i);
            }

            for (int i = 0; i < functors; i++)
            {
                Functors[i] = new UnknownFunctor(i);
            }
        }
    }
}
