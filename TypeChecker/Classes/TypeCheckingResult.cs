using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Typechecker.Helpers;

namespace Purity.Compiler.Typechecker.Classes
{
    public class TypeCheckingResult
    {
        public IPartialType Type
        {
            get;
            set;
        }

        public IConstrainedData Data
        {
            get;
            set;
        }

        public IEnumerable<Constraint> Constraints
        {
            get;
            set;
        }

        public int MaximalIndex
        {
            get;
            set;
        }

        public TypeCheckingResult(IConstrainedData data, IPartialType type, 
            IEnumerable<Constraint> constraints, int maximalIndex)
        {
            Type = type;
            Data = data;
            Constraints = constraints;
            MaximalIndex = maximalIndex;
        }
    }
}
