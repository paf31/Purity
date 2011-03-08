using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core.Functions
{
    public class Cl<A, B, C> : IFunction<IFunction<B, C>, IFunction<A, C>>
    {
        private readonly IFunction<A, B> function;

        public Cl(IFunction<A, B> function)
        {
            this.function = function;
        }

        public IFunction<A, C> Call(IFunction<B, C> a)
        {
            return new CompositeFunction<A, B, C>(function, a);
        }
    }
}
