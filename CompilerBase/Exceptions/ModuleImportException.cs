using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Exceptions
{
    public class ModuleImportException : Exception
    {
        public ModuleImportException()
            : base()
        {
        }

        public ModuleImportException(string message)
            : base(message)
        {
        }

        public ModuleImportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
