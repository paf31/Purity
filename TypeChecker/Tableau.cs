using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Typechecker.Types;
using Purity.Compiler.Typechecker.Functors;
using Purity.Compiler.Interfaces;

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

        public IDictionary<int, Tuple<int, int>> FunctorApplications
        {
            get;
            set;
        }

        public IList<Tuple<int, int>> Collisions
        {
            get;
            set;
        }

        public void AddCollision(int i1, int i2)
        {
            Tuple<int, int> tuple;

            if (i1 < i2)
            {
                tuple = Tuple.Create(i1, i2);
            }
            else
            {
                tuple = Tuple.Create(i2, i1);
            }

            if (!Collisions.Contains(tuple))
            {
                Collisions.Add(tuple);
            }
        }

        public Tableau()
        {
            Types = new Dictionary<int, IPartialType>();
            Functors = new Dictionary<int, IPartialFunctor>();
            FunctorApplications = new Dictionary<int, Tuple<int, int>>();
            Collisions = new List<Tuple<int, int>>();
        }

        public Tableau(int types, int functors)
            : this()
        {
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
