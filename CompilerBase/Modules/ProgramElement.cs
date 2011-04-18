using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Modules
{
    public class ProgramElement
    {
        public ProgramElementType ElementType { get; set; }

        public Named<FunctorDeclaration> Functor { get; set; }
        
        public Named<ITypeDeclaration> Type { get; set; }
        
        public Named<DataDeclaration> Data { get; set; }

        public ProgramElement(Named<FunctorDeclaration> functor)
        {
            ElementType = ProgramElementType.Functor;
            Functor = functor;
        }

        public ProgramElement(Named<ITypeDeclaration> type)
        {
            ElementType = ProgramElementType.Type;
            Type = type;
        }

        public ProgramElement(Named<DataDeclaration> data)
        {
            ElementType = ProgramElementType.Data;
            Data = data;
        }
    }
}
