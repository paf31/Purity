using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core.Functions
{
    public static class DelegateFunction
    {
        public static IFunction<A, B> Create<A, B>(Func<A, B> f)
        {
            return new DelegateFunction<A, B>(f);
        }
    }

    public class DelegateFunction<A, B> : IFunction<A, B>
    {
        private readonly Func<A, B> f;

        public DelegateFunction(Func<A, B> f)
        {
            this.f = f;
        }

        public B Call(A a)
        {
            return f(a);
        }
    }
}
