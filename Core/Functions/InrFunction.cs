using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Core.Types;

namespace Purity.Core.Functions
{
    public class InrFunction<A, B> : IFunction<B, Either<A, B>>
    {
        public Either<A, B> Call(B b)
        {
            return Either<A, B>.Inr(b);
        }
    }
}
