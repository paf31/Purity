using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Types;
using Purity.Compiler.TypeDeclarations;

namespace Purity.Compiler.Interfaces
{
    public interface ITypeDeclarationVisitor
    {
        void VisitBox(BoxedTypeDeclaration t);

        void VisitLFix(LFixTypeDeclaration t);

        void VisitGFix(GFixTypeDeclaration t);
    }

    public interface ITypeDeclarationVisitor<R>
    {
        R VisitBox(BoxedTypeDeclaration t);

        R VisitLFix(LFixTypeDeclaration t);

        R VisitGFix(GFixTypeDeclaration t);
    }
}
