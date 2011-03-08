using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core.Functions
{
    public class CompositeFunction<A, B, C> : IFunction<A, C>
    {
        private readonly IFunction<A, B> f1;
        private readonly IFunction<B, C> f2;

        public CompositeFunction(IFunction<A, B> f1, IFunction<B, C> f2)
        {
            this.f1 = f1;
            this.f2 = f2;
        }

        public C Call(A a)
        {
            return f2.Call(f1.Call(a));
        }
    }
}
