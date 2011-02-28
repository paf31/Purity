using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core
{
    public class InlFunction<A, B> : IFunction<A, Either<A, B>>
    {
        public Either<A, B> Call(A a)
        {
            return Either<A, B>.Inl(a);
        }
    }
}
