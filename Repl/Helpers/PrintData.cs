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
using Purity.Compiler.Types;

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
            else 
            {
                return declaration.AcceptVisitor(new PrintSynonymVisitor(t, data, typeInfo, maxDepth));
            }
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

        private class PrintSynonymVisitor : ITypeDeclarationVisitor<string>
        {
            private readonly Type type;
            private readonly int maxDepth;
            private readonly dynamic data;
            private readonly TypeSynonym synonym;

            public PrintSynonymVisitor(TypeSynonym synonym, dynamic data, Type type, int maxDepth)
            {
                this.synonym = synonym;
                this.data = data;
                this.type = type;
                this.maxDepth = maxDepth;
            }

            public string VisitBox(BoxedTypeDeclaration t)
            {
                Type destructor = DataContainer.ResolveDestructor(synonym.Identifier);

                dynamic destructorDelegate = Activator.CreateInstance(destructor);
                dynamic unboxed = destructorDelegate.Call(data);

                return string.Format("{0} ({1})", t.ConstructorFunctionName ?? "_",
                    PrintData.Print(unboxed, t.Type, maxDepth));
            }

            public string VisitLFix(LFixTypeDeclaration t)
            {
                Type destructor = DataContainer.ResolveDestructor(synonym.Identifier);

                dynamic destructorDelegate = Activator.CreateInstance(destructor);
                dynamic unboxed = destructorDelegate.Call(data);

                var flfix = FunctorApplication.Map(t.Functor, synonym);
                return string.Format("{0} ({1})", t.ConstructorFunctionName ?? "_",
                    PrintData.Print(unboxed, flfix, maxDepth - 1));
            }

            public string VisitGFix(GFixTypeDeclaration t)
            {
                Type destructor = DataContainer.ResolveDestructor(synonym.Identifier);

                dynamic destructorDelegate = Activator.CreateInstance(destructor);
                dynamic unboxed = destructorDelegate.Call(data);

                var flfix = FunctorApplication.Map(t.Functor, synonym);
                return string.Format("{0} ({1})", t.ConstructorFunctionName ?? "_",
                    PrintData.Print(unboxed, flfix, maxDepth - 1));
            }
        }
    }
}
