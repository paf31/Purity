using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core
{
    public class ArrowFunctorAction<T, A, B> : IFunction<IFunction<T, A>, IFunction<T, B>>
    {
        private readonly IFunction<A, B> f;
        
        public ArrowFunctorAction(IFunction<A, B> f)
        {
            this.f = f;
        }

        public IFunction<T, B> Call(IFunction<T, A> a)
        {
            return new CompositeFunction<T, A, B>(a, f);
        }
    }
}
