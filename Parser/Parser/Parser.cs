using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Parser
{
    public delegate Result<I, O> Parser<I, O>(I input);
}
