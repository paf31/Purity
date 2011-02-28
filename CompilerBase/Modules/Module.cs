using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Data;
using Purity.Compiler.TypedExpressions;

namespace Purity.Compiler.Modules
{
    public class Module
    {
        public IEnumerable<ProgramElement> Elements { get; set; }

        public Module(IEnumerable<ProgramElement> elements)
        {
            Elements = elements;
        }
    }
}
