using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.TypeDeclarations
{
    public class GFixTypeDeclaration : ITypeDeclaration
    {
        public GFixTypeDeclaration(IType type, string variableName, string[] typeParameters,
            string constructorFunctionName, string destructorFunctionName, string anaFunctionName)
        {
            Type = type;
            VariableName = variableName;
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
            visitor.VisitGFix(this);
        }

        public R AcceptVisitor<R>(ITypeDeclarationVisitor<R> visitor)
        {
            return visitor.VisitGFix(this);
        }
    }
}
