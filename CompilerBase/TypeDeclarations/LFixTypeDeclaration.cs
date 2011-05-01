using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypeDeclarations
{
    public class LFixTypeDeclaration : ITypeDeclaration
    {
        public LFixTypeDeclaration(IType type, string variableName, string[] typeParameters, 
            string constructorFunctionName, string destructorFunctionName, string cataFunctionName)
        {
            Type = type;
            VariableName = variableName;
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

        public IType Type
        {
            get;
            set;
        }

        public string VariableName
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
