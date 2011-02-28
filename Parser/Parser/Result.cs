using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Parser
{
    public class Result<I, O>
    {
        public I Rest { get; set; }

        public O Output { get; set; }

        public Result(O output, I rest)
        {
            Output = output;
            Rest = rest;
        }
    }
}
