using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Modules
{
    public class Named<T>
    {
        public string Name
        {
            get;
            set;
        }

        public T Value
        {
            get;
            set;
        }

        public Named(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }
}
