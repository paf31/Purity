using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

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

        public DataDeclaration(IType type, IData data)
        {
            Type = type;
            Data = data;
        }
    }
}
