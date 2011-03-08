using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core.Functions
{
    public class OutlFunction<A, B> : IFunction<Tuple<A, B>, A>
    {
        public A Call(Tuple<A, B> p)
        {
            return p.Item1;
        }
    }
}
