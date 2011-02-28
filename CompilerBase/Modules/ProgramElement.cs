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

        public Named<IFunctor> Functor { get; set; }
        
        public Named<IType> Type { get; set; }
        
        public Named<DataDeclaration> Data { get; set; }

        public ProgramElement(Named<IFunctor> functor)
        {
            ElementType = ProgramElementType.Functor;
            Functor = functor;
        }

        public ProgramElement(Named<IType> type)
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
