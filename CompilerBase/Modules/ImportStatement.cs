using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Modules
{
    public class ImportStatement
    {
        public string ModuleName
        {
            get;
            set;
        }

        public ImportStatement(string moduleName)
        {
            ModuleName = moduleName;
        }
    }
}
