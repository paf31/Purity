using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Core.Types;

namespace Purity.Compiler.Helpers
{
    public class FunctorApplication : ITypeVisitor<IType>
    {
        private readonly IType type;
        private string variableName;

        public FunctorApplication(string variableName, IType type)
        {
            this.type = type;
            this.variableName = variableName;
        }

        public static IType Map(string variableName, IType functorType, IType type)
        {
            var visitor = new FunctorApplication(variableName, type);
            return functorType.AcceptVisitor(visitor);
        }

        public IType VisitArrow(Types.ArrowType t)
        {
            return new Types.ArrowType(t.Left.AcceptVisitor(this), t.Right.AcceptVisitor(this));
        }

        public IType VisitSynonym(Types.TypeSynonym t)
        {
            return t;
        }

        public IType VisitProduct(Types.ProductType t)
        {
            return new Types.ProductType(t.Left.AcceptVisitor(this), t.Right.AcceptVisitor(this));
        }

        public IType VisitSum(Types.SumType t)
        {
            return new Types.SumType(t.Left.AcceptVisitor(this), t.Right.AcceptVisitor(this));
        }

        public IType VisitParameter(Types.TypeParameter t)
        {
            if (t.Identifier.Equals(variableName))
            {
                return type;
            }
            else 
            {
                return t; 
            }
        }
    }
}
