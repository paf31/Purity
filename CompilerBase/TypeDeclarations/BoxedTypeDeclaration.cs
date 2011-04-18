using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypeDeclarations
{
    public class BoxedTypeDeclaration : ITypeDeclaration
    {
        public BoxedTypeDeclaration(IType type, string[] typeParameters,
            string constructorFunctionName, string destructorFunctionName)
        {
            Type = type;
            TypeParameters = typeParameters;
            ConstructorFunctionName = constructorFunctionName;
            DestructorFunctionName = destructorFunctionName;
        }

        public string[] TypeParameters
        {
            get;
            set;
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

        public IType Type
        {
            get;
            set;
        }

        public void AcceptVisitor(ITypeDeclarationVisitor visitor)
        {
            visitor.VisitBox(this);
        }

        public R AcceptVisitor<R>(ITypeDeclarationVisitor<R> visitor)
        {
            return visitor.VisitBox(this);
        }
    }
}
