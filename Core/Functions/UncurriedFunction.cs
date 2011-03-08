using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core.Functions
{
    public class UncurriedFunction<A, B, C> : IFunction<Tuple<A, B>, C>
    {
        private readonly IFunction<A, IFunction<B, C>> f;

        public UncurriedFunction(IFunction<A, IFunction<B, C>> f)
        {
            this.f = f;
        }

        public C Call(Tuple<A, B> a)
        {
            return f.Call(a.Item1).Call(a.Item2);
        }
    }
}
