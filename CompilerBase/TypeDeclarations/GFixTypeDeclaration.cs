using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypeDeclarations
{
    public class GFixTypeDeclaration : ITypeDeclaration
    {
        public GFixTypeDeclaration(IFunctor functor, string[] typeParameters,
            string constructorFunctionName, string destructorFunctionName, string anaFunctionName)
        {
            Functor = functor;
            TypeParameters = typeParameters;
            ConstructorFunctionName = constructorFunctionName;
            DestructorFunctionName = destructorFunctionName;
            AnaFunctionName = anaFunctionName;
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

        public string AnaFunctionName
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
            visitor.VisitGFix(this);
        }

        public R AcceptVisitor<R>(ITypeDeclarationVisitor<R> visitor)
        {
            return visitor.VisitGFix(this);
        }
    }
}
