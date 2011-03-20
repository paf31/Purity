using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Types;

namespace Purity.Compiler.Modules
{
    public class DataDeclaration
    {
        public IData Data
        {
            get;
            set;
        }

        public IType Type
        {
            get;
            set;
        }

        public string[] TypeParameters
        {
            get;
            set;
        }

        public DataDeclaration(IType type, IData data)
        {
            Type = type;
            Data = data;
        }

        public DataDeclaration(IData data)
            : this(null, data)
        {
        }
    }
}
