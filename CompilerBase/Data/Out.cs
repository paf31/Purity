﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Data
{
    public class Out : IData
    {
        public void AcceptVisitor(IDataVisitor visitor)
        {
            visitor.VisitOut(this);
        }

        public IType Type
        {
            get;
            set;
        }
    }
}