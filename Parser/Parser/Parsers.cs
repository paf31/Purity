using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler.Parser
{
    public static class Parsers
    {
        public static Parser<string, char> WSChar = Parsers.Match(' ').Or(Parsers.Match('\n')).Or(Parsers.Match('\r'));

        public static Parser<string, IEnumerable<char>> Whitespace = WSChar.Rep();

        private static Parser<string, string> AlphaNumeric = from first in Any().Where(char.IsLetter)
                                                          from rest in Any().Where(char.IsLetterOrDigit).Rep()
                                                          select new string(new[] { first }.Concat(rest).ToArray());

        public static Parser<string, string> Identifier = AlphaNumeric.Where(Keywords.IsNotKeyword);

        public static Parser<I, O> Return<I, O>(O output)
        {
            return i => new Result<I, O>(output, i);
        }

        public static Parser<I, O> Fail<I, O>()
        {
            return i => null;
        }

        public static Parser<I, O1> Select<I, O, O1>(this Parser<I, O> p, Func<O, O1> f)
        {
            return i =>
            {
                var result = p(i);
                return result == null ? null : new Result<I, O1>(f(result.Output), result.Rest);
            };
        }

        public static Parser<I, O1> SelectMany<I, O, O1>(this Parser<I, O> p, Func<O, Parser<I, O1>> f)
        {
            return i =>
            {
                var result = p(i);
                return result == null ? null : f(result.Output)(result.Rest);
            };
        }

        public static Parser<I, O2> SelectMany<I, O, O1, O2>(this Parser<I, O> p, Func<O, Parser<I, O1>> f, Func<O, O1, O2> proj)
        {
            return p.SelectMany(o => f(o).Select(o1 => proj(o, o1)));
        }

        public static Parser<I, O> Where<I, O>(this Parser<I, O> p, Func<O, bool> pred)
        {
            return p.SelectMany(o => pred(o) ? Return<I, O>(o) : Fail<I, O>());
        }

        public static Parser<I, O1> Then<I, O, O1>(this Parser<I, O> p1, Parser<I, O1> p2)
        {
            return p1.SelectMany(o => p2);
        }

        public static Parser<I, O> Or<I, O>(this Parser<I, O> p1, Parser<I, O> p2)
        {
            return i => p1(i) ?? p2(i);
        }

        public static Parser<I, IEnumerable<O>> Rep<I, O>(this Parser<I, O> p)
        {
            return (from o in p
                    from os in p.Rep()
                    select new[] { o }.Concat(os)).Or(Return<I, IEnumerable<O>>(Enumerable.Empty<O>()));
        }

        public static Parser<I, IEnumerable<O>> Rep1<I, O>(this Parser<I, O> p)
        {
            return from o in p
                   from os in Rep(p)
                   select new[] { o }.Concat(os);
        }

        public static Parser<string, char> Any()
        {
            return s => string.IsNullOrEmpty(s) ? null : new Result<string, char>(s[0], s.Substring(1));
        }

        public static Parser<string, char> Match(char c)
        {
            return Any().Where(c1 => c == c1);
        }

        public static Parser<string, string> Match(string s)
        {
            return s.Select(Match).Aggregate(Return<string, string>(string.Empty), (p1, p2) => from cs in p1
                                                                                               from c in p2
                                                                                               select cs + c);
        }
    }
}
