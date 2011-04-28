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
        public string Name
        {
            get;
            set;
        }

        public IEnumerable<ProgramElement> Elements
        {
            get;
            set;
        }

        public Module(string name, IEnumerable<ProgramElement> elements)
        {
            Name = name;
            Elements = elements;
        }
    }
}
