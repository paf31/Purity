using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Modules;
using Purity.Compiler.Data;
using Purity.Compiler.TypedExpressions;

namespace Purity.Compiler.Extensions
{
    public static class ExtensionMethods
    {
        public static IData RemoveSynonyms(this IData d)
        {
            var visitor = new DataSynonymRemover();
            d.AcceptVisitor(visitor);
            return visitor.Result;
        }

        public static Named<IData> RemoveSynonyms(this Named<IData> d)
        {
            return new Named<IData>(d.Name, d.Value.RemoveSynonyms());
        }

        public static Named<DataDeclaration> RemoveSynonyms(this Named<DataDeclaration> d)
        {
            return new Named<DataDeclaration>(d.Name, new DataDeclaration(d.Value.Type.RemoveSynonyms(), d.Value.Data.RemoveSynonyms()));
        }

        public static IType RemoveSynonyms(this IType t)
        {
            var visitor = new TypeSynonymRemover();
            t.AcceptVisitor(visitor);
            return visitor.Result;
        }

        public static Named<IType> RemoveSynonyms(this Named<IType> t)
        {
            return new Named<IType>(t.Name, t.Value.RemoveSynonyms());
        }

        public static IFunctor RemoveSynonyms(this IFunctor f)
        {
            var visitor = new FunctorSynonymRemover();
            f.AcceptVisitor(visitor);
            return visitor.Result;
        }

        public static Named<IFunctor> RemoveSynonyms(this Named<IFunctor> f)
        {
            return new Named<IFunctor>(f.Name, f.Value.RemoveSynonyms());
        }
    }
}
