using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler;
using Purity.Compiler.Data;
using Purity.Core;
using System.Reflection.Emit;
using System.Reflection;
using Purity.Compiler.Helpers;
using Purity.Compiler.TypeDeclarations;
using Purity.Compiler.Exceptions;

namespace Repl.Helpers
{
    public class PrintData : ITypeVisitor<string>
    {
        private readonly dynamic data;
        private readonly int maxDepth;

        public PrintData(dynamic data, int maxDepth)
        {
            this.data = data;
            this.maxDepth = maxDepth;
        }

        public static string Print(dynamic data, IType t, int maxDepth)
        {
            return t.AcceptVisitor(new PrintData(data, maxDepth));
        }

        public string VisitArrow(Purity.Compiler.Types.ArrowType t)
        {
            return "?";
        }

        public string VisitSynonym(Purity.Compiler.Types.TypeSynonym t)
        {
            var typeInfo = TypeContainer.ResolveType(t.Identifier);
            var declaration = Container.ResolveType(t.Identifier);

            if (maxDepth <= 0)
            {
                return "...";
            }
            else if (typeInfo is BoxedTypeInfo)
            {
                dynamic unboxFunction = Activator.CreateInstance((typeInfo as BoxedTypeInfo).UnboxFunction);
                dynamic unboxed = unboxFunction.Call(data);

                var type = (declaration as BoxedTypeDeclaration).Type;

                return string.Format("[{0}]", Print(unboxed, type, maxDepth));
            }
            else if (typeInfo is LFixTypeInfo)
            {
                dynamic inFunction = Activator.CreateInstance((typeInfo as LFixTypeInfo).InFunction);
                dynamic unpacked = inFunction.Call(data);

                var functor = (declaration as LFixTypeDeclaration).Functor;
                var flfix = FunctorApplication.Map(functor, t);
                return string.Format("out ({0})", Print(unpacked, flfix, maxDepth - 1));
            }
            else if (typeInfo is GFixTypeInfo)
            {
                dynamic inFunction = Activator.CreateInstance((typeInfo as GFixTypeInfo).InFunction);
                dynamic unpacked = inFunction.Call(data);

                var functor = (declaration as GFixTypeDeclaration).Functor;
                var flfix = FunctorApplication.Map(functor, t);
                return string.Format("out ({0})", Print(unpacked, flfix, maxDepth - 1));
            }

            throw new CompilerException("Unknown type declaration.");
        }

        public string VisitProduct(Purity.Compiler.Types.ProductType t)
        {
            return string.Format("({0},{1})", Print(data.Item1, t.Left, maxDepth), Print(data.Item2, t.Right, maxDepth));
        }

        public string VisitSum(Purity.Compiler.Types.SumType t)
        {
            return data.Case(
                (Func<dynamic, string>)(l => string.Format("(inl {0})", Print(l, t.Left, maxDepth))),
                (Func<dynamic, string>)(r => string.Format("(inr {0})", Print(r, t.Right, maxDepth))));
        }

        public string VisitParameter(Purity.Compiler.Types.TypeParameter t)
        {
            throw new NotSupportedException();
        }
    }
}
