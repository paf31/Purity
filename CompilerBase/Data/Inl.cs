﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Data
{
    public class Inl : IData
    {
        public void AcceptVisitor(IDataVisitor visitor)
        {
            visitor.VisitInl(this);
        }

        public IType Type
        {
            get;
            set;
        }
    }
}