using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core.Functions
{
    public class SplitFunction<A, B, C> : IFunction<A, Tuple<B, C>>
    {
        private readonly IFunction<A, B> f1;
        private readonly IFunction<A, C> f2;

        public SplitFunction(IFunction<A, B> f1, IFunction<A, C> f2)
        {
            this.f1 = f1;
            this.f2 = f2;
        }

        public Tuple<B, C> Call(A a)
        {
            return Tuple.Create(f1.Call(a), f2.Call(a));
        }
    }
}
