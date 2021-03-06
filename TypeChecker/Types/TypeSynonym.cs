﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Types
{
    public class TypeSynonym : IPartialType
    {
        public string Identifier { get; set; }

        public IPartialType[] TypeParameters { get; set; }

        public TypeSynonym(string identifier, IPartialType[] typeParameters)
        {
            Identifier = identifier;
            TypeParameters = typeParameters;
        }

        public void AcceptVisitor(IPartialTypeVisitor visitor)
        {
            visitor.VisitSynonym(this);
        }

        public R AcceptVisitor<R>(IPartialTypeVisitor<R> visitor)
        {
            return visitor.VisitSynonym(this);
        }
    }
}
