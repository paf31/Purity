using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypeDeclarations
{
    public class LFixTypeDeclaration : ITypeDeclaration
    {
        public LFixTypeDeclaration(IFunctor functor, string[] typeParameters, 
            string constructorFunctionName, string destructorFunctionName, string cataFunctionName)
        {
            Functor = functor;
            TypeParameters = typeParameters;
            ConstructorFunctionName = constructorFunctionName;
            DestructorFunctionName = destructorFunctionName;
            CataFunctionName = cataFunctionName;
        }

        public string ConstructorFunctionName
        {
            get;
            set;
        }

        public string DestructorFunctionName
        {
            get;
            set;
        }

        public string CataFunctionName
        {
            get;
            set;
        }

        public string[] TypeParameters
        {
            get;
            set;
        }

        public IFunctor Functor
        {
            get;
            set;
        }

        public void AcceptVisitor(ITypeDeclarationVisitor visitor)
        {
            visitor.VisitLFix(this);
        }

        public R AcceptVisitor<R>(ITypeDeclarationVisitor<R> visitor)
        {
            return visitor.VisitLFix(this);
        }
    }
}
