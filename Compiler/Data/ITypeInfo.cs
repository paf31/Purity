using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace Purity.Compiler.Data
{
    public interface ITypeInfo
    {
        TypeBuilder Type
        {
            get;
            set;
        }
    }
}
