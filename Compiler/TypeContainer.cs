using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Data;
using System.Reflection;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler
{
    public static class TypeContainer
    {
        private static IDictionary<IFunctor, LFixTypeInfo> lfixTypes =
            new Dictionary<IFunctor, LFixTypeInfo>();

        private static IDictionary<IFunctor, GFixTypeInfo> gfixTypes =
            new Dictionary<IFunctor, GFixTypeInfo>();

        public static LFixTypeInfo ResolveLFixType(IFunctor functor)
        {
            if (!lfixTypes.ContainsKey(functor))
            {
                throw new CompilerException("Unable to resolve least fixed point type " + functor.ToString()); 
            }

            return lfixTypes[functor];
        }

        public static GFixTypeInfo ResolveGFixType(IFunctor functor)
        {
            if (!lfixTypes.ContainsKey(functor))
            {
                throw new CompilerException("Unable to resolve greatest fixed point type " + functor.ToString());
            }

            return gfixTypes[functor];
        }

        public static void Add(IFunctor functor, LFixTypeInfo value)
        {
            lfixTypes[functor] = value;
        }

        public static void Add(IFunctor functor, GFixTypeInfo value)
        {
            gfixTypes[functor] = value;
        }

        public static bool HasLFixType(IFunctor functor)
        {
            return lfixTypes.ContainsKey(functor);
        }

        public static bool HasGFixType(IFunctor functor)
        {
            return gfixTypes.ContainsKey(functor);
        }
    }
}
