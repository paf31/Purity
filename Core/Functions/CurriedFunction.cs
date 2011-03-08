using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core.Functions
{
    public class CurriedFunction<A, B, C> : IFunction<A, IFunction<B, C>>
    {
        private readonly IFunction<Tuple<A, B>, C> f;

        public CurriedFunction(IFunction<Tuple<A, B>, C> f)
        {
            this.f = f;
        }

        public IFunction<B, C> Call(A a)
        {
            return new CompositeFunction<B, Tuple<A, B>, C>(new SplitFunction<B, A, B>(new ConstantFunction<B, A>(a), new IdentityFunction<B>()), f);
        }
    }
}
