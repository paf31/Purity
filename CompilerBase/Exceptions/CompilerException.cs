using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Exceptions
{
    public class CompilerException : Exception
    {
        public CompilerException() : base()
        {
        }

        public CompilerException(string message)
            : base(message)
        {
        }

        public CompilerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
