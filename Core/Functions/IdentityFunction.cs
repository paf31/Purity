﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core.Functions
{
    public class IdentityFunction<A> : IFunction<A, A>
    {
        public A Call(A a)
        {
            return a;
        }
    }
}
