using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core
{
    public interface IFunction<A, B>
    {
        B Call(A a);
    }
}
