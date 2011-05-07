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
        private readonly IMetadataContainer container;
        private readonly IRuntimeContainer runtimeContainer;

        public PrintData(dynamic data, int maxDepth, IMetadataContainer container,
            IRuntimeContainer runtimeContainer)
        {
            this.data = data;
            this.maxDepth = maxDepth;
            this.container = container;
            this.runtimeContainer = runtimeContainer;
        }

        public static string Print(dynamic data, IType t, int maxDepth, IMetadataContainer container,
            IRuntimeContainer runtimeContainer)
        {
            return t.AcceptVisitor(new PrintData(data, maxDepth, container, runtimeContainer));
        }

        public string VisitArrow(Purity.Compiler.Types.ArrowType t)
        {
            return "?";
        }

        public string VisitSynonym(Purity.Compiler.Types.TypeSynonym t)
        {
            var type = runtimeContainer.ResolveType(t.Identifier);
            var declaration = container.ResolveType(t.Identifier);

            if (maxDepth <= 0)
            {
                return "...";
            }
            else
            {
                Type destructor = runtimeContainer.ResolveDestructor(t.Identifier);

                if (destructor is TypeBuilder)
                {
                    destructor = ((TypeBuilder)destructor).CreateType();
                }

                if (destructor.GetGenericArguments().Any())
                {
                    Type dataType = data.GetType();
                    Type interfaceType = dataType.GetInterface(type.Name);

                    destructor = destructor.MakeGenericType(interfaceType.GetGenericArguments());
                }

                dynamic destructorInstance = Activator.CreateInstance(destructor);
                dynamic unboxed = destructorInstance.Call(data);

                return declaration.AcceptVisitor(new PrintSynonymVisitor(unboxed, maxDepth, t, container, runtimeContainer));
            }
        }

        public string VisitProduct(Purity.Compiler.Types.ProductType t)
        {
            return string.Format("({0}, {1})",
                Print(data.Item1, t.Left, maxDepth, container, runtimeContainer),
                Print(data.Item2, t.Right, maxDepth, container, runtimeContainer));
        }

        public string VisitSum(Purity.Compiler.Types.SumType t)
        {
            return data.Case(
                (Func<dynamic, string>)(l => string.Format("inl {0}",
                    Print(l, t.Left, maxDepth, container, runtimeContainer))),
                (Func<dynamic, string>)(r => string.Format("inr {0}",
                    Print(r, t.Right, maxDepth, container, runtimeContainer))));
        }

        public string VisitParameter(Purity.Compiler.Types.TypeParameter t)
        {
            throw new NotSupportedException();
        }

        private class PrintSynonymVisitor : ITypeDeclarationVisitor<string>
        {
            private readonly dynamic unboxed;
            private readonly int maxDepth;
            private readonly TypeSynonym synonym;
            private readonly IMetadataContainer container;
            private readonly IRuntimeContainer runtimeContainer;

            public PrintSynonymVisitor(dynamic unboxed, int maxDepth, TypeSynonym synonym, IMetadataContainer container,
                IRuntimeContainer runtimeContainer)
            {
                this.unboxed = unboxed;
                this.maxDepth = maxDepth;
                this.synonym = synonym;
                this.container = container;
                this.runtimeContainer = runtimeContainer;
            }

            public string VisitBox(BoxedTypeDeclaration t)
            {
                return string.Format("{0} {1}", t.ConstructorFunctionName ?? "_",
                    PrintData.Print(unboxed, t.Type, maxDepth, container, runtimeContainer));
            }

            public string VisitLFix(LFixTypeDeclaration t)
            {
                var typeParameters = t.TypeParameters
                    .Select((s, i) => new KeyValuePair<string, IType>(s, synonym.TypeParameters[i]))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);

                var flfix = ReplaceTypeParameters.Replace(FunctorApplication.Map(t.VariableName, t.Type, synonym), typeParameters);
                return string.Format("{0} {1}", t.ConstructorFunctionName ?? "_",
                    PrintData.Print(unboxed, flfix, maxDepth - 1, container, runtimeContainer));
            }

            public string VisitGFix(GFixTypeDeclaration t)
            {
                var typeParameters = t.TypeParameters
                    .Select((s, i) => new KeyValuePair<string, IType>(s, synonym.TypeParameters[i]))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);

                var flfix = ReplaceTypeParameters.Replace(FunctorApplication.Map(t.VariableName, t.Type, synonym), typeParameters);
                return string.Format("{0} {1}", t.ConstructorFunctionName ?? "_",
                    PrintData.Print(unboxed, flfix, maxDepth - 1, container, runtimeContainer));
            }
        }
    }
}
