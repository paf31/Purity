using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core.Functions
{
    public class OutrFunction<A, B> : IFunction<Tuple<A, B>, B>
    {
        public B Call(Tuple<A, B> p)
        {
            return p.Item2;
        }
    }
}
