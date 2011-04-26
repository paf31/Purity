using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Classes
{
    public class SolutionSet
    {
        private IDictionary<int, IPartialType> types = new Dictionary<int, IPartialType>();

        public IPartialType this[int index]
        {
            get
            {
                IPartialType type;

                if (!types.TryGetValue(index, out type))
                {
                    type = new Types.UnknownType(index);
                }

                return type;
            }
            set
            {
                types[index] = value;
            }
        }
    }
}
