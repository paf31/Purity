﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Core;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Types
{
    public class ArrowType : IType
    {
        public IType Left { get; set; }

        public IType Right { get; set; }

        public ArrowType(IType left, IType right)
        {
            Left = left;
            Right = right;
        }

        public void AcceptVisitor(ITypeVisitor visitor)
        {
            visitor.VisitArrow(this);
        }

        public R AcceptVisitor<R>(ITypeVisitor<R> visitor)
        {
            return visitor.VisitArrow(this);
        }
    }
}
