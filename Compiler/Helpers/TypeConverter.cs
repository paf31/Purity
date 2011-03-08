using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Extensions;
using Purity.Compiler.Interfaces;
using Purity.Core;
using System.Reflection.Emit;
using Purity.Compiler.Functors;
using Purity.Compiler.Data;
using Purity.Core.Types;

namespace Purity.Compiler.Helpers
{
    public class TypeConverter : ITypeVisitor
    {
        public Type Result
        {
            get;
            set;
        }

        public Type Convert(IType t)
        {
            t.AcceptVisitor(this);
            return Result;
        }

        public void VisitArrow(Types.ArrowType t)
        {
            Result = typeof(IFunction<,>).MakeGenericType(Convert(t.Left), Convert(t.Right));
        }

        public void VisitProduct(Types.ProductType t)
        {
            Result = typeof(Tuple<,>).MakeGenericType(Convert(t.Left), Convert(t.Right));
        }

        public void VisitSum(Types.SumType t)
        {
            Result = typeof(Either<,>).MakeGenericType(Convert(t.Left), Convert(t.Right));
        }

        public void VisitSynonym(Types.TypeSynonym t)
        {
            ITypeInfo typeInfo = TypeContainer.ResolveType(t.Identifier);
            Result = typeInfo.Type;
        }

        public void VisitLFix(Types.LFixType t)
        {
            Result = TypeContainer.ResolveLFixType(t.Identifier).Type;
        }

        public void VisitGFix(Types.GFixType t)
        {
            Result = TypeContainer.ResolveGFixType(t.Identifier).Type;
        }
    }
}
