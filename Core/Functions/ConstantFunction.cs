using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core.Functions
{
    public class ConstantFunction<A, B> : IFunction<A, B>
    {
        private readonly B value;

        public ConstantFunction(B value)
        {
            this.value = value;
        }

        public B Call(A a)
        {
            return value;
        }
    }
}
