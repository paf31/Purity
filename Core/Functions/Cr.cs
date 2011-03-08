using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core.Functions
{
    public class Cr<A, B, C> : IFunction<IFunction<A, B>, IFunction<A, C>>
    {
        private readonly IFunction<B, C> function;

        public Cr(IFunction<B, C> function)
        {
            this.function = function;
        }

        public IFunction<A, C> Call(IFunction<A, B> a)
        {
            return new CompositeFunction<A, B, C>(a, function);
        }
    }
}
