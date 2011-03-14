using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler
{
    public static class Keywords
    {
        public static readonly string[] KeywordList = new[] 
        {
            Constants.DataKeyword,
            Constants.FunctorKeyword,
            Constants.TypeKeyword,
            Constants.In,
            Constants.Out,
            Constants.Inl,
            Constants.Inr,
            Constants.Outl,
            Constants.Outr,
            Constants.Const,
            Constants.Id,
            Constants.Lfix,
            Constants.Gfix,
            Constants.Curry,
            Constants.Uncurry
        };

        public static bool IsKeyword(string s)
        {
            return KeywordList.Contains(s);
        }

        public static bool IsNotKeyword(string s)
        {
            return !IsKeyword(s);
        }
    }
}
