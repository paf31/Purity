using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler
{
    public static class Constants
    {
        public const char CompositionOperator = '.';

        public const char EqualsSymbol = '=';

        public const char OpeningBracket = '(';
        public const char ClosingBracket = ')';

        public const char OpeningSquareBracket = '[';
        public const char ClosingSquareBracket = ']';

        public const char CaseOpeningBrace = '<';
        public const char CaseClosingBrace = '>';
        public const char CaseSeparator = ',';

        public const char SplitOpeningBrace = '(';
        public const char SplitClosingBrace = ')';
        public const char SplitSeparator = ',';

        public const string AnaOpeningBrace = "{|";
        public const string AnaClosingBrace = "|}";

        public const string CataOpeningBrace = "(|";
        public const string CataClosingBrace = "|)";

        public const string Inl = "inl";
        public const string Inr = "inr";

        public const string Outl = "outl";
        public const string Outr = "outr";

        public const string Const = "const";

        public const string ComposeOnTheLeft = "cl";
        public const string ComposeOnTheRight = "cr";

        public const string Id = "id";

        public const string In = "in";
        public const string Out = "out";

        public const string Curry = "curry";
        public const string Uncurry = "uncurry";

        public const string FunctorKeyword = "functor";
        public const string TypeKeyword = "type";
        public const string DataKeyword = "data";

        public const char TypeOperatorSum = '+';
        public const char TypeOperatorProduct = '.';
        public const string TypeOperatorArrow = "->";

        public const char FunctorOperatorSum = '+';
        public const char FunctorOperatorProduct = '.';
        public const string FunctorOperatorArrow = "->";

        public const string Lfix = "lfix";
        public const string Gfix = "gfix";
    }
}
