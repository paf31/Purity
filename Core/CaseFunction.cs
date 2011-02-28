using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core
{
    public class CaseFunction<A, B, C> : IFunction<Either<A, B>, C>
    {
        private readonly IFunction<A, C> f1;
        private readonly IFunction<B, C> f2;

        public CaseFunction(IFunction<A, C> f1, IFunction<B, C> f2)
        {
            this.f1 = f1;
            this.f2 = f2;
        }

        public C Call(Either<A, B> a)
        {
            return a.Case(f1.Call, f2.Call);
        }
    }
}
