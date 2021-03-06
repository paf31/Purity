﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Interfaces
{
    public interface ITypeDeclaration
    {
        void AcceptVisitor(ITypeDeclarationVisitor visitor);

        R AcceptVisitor<R>(ITypeDeclarationVisitor<R> visitor);
    }
}
