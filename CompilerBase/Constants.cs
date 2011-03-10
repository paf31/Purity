using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler
{
    public static class Constants
    {
        public const char StartOfComment = '#';

        public const char CompositionOperator = '.';

        public const char EqualsSymbol = '=';

        public const char OpeningBracket = '(';
        public const char ClosingBracket = ')';

        public const char DataTypeIntroduction = ':';

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

        public const char Box = '&';
        public const char Unbox = '^';

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

        public const string CallMethodName = "Call";
        
        public const string MethodsSuffix = "Methods";

        public const string BoxFunctionClassName = "BoxFunction";
        public const string UnboxFunctionClassName = "UnboxFunction";
        public const string BoxedTypeValueFieldName = "Value";

        public const string DataClassName = "Data";
        public const string TypesNamespace = "Types";

        public const string CataMethodName = "Cata";
        public const string CataMethodGenericParameterName = "T";

        public const string ApplyMethodName = "Apply";
        public const string ApplyMethodGenericParameterName = "T";

        public const string GFixFunctionClassGenericParameterName = "R";

        public const string AnaMethodName = "Ana";
        public const string AnaMethodGenericParameterName = "T";

        public const string FMapMethodName = "FMap";
        public const string FMapMethodInputParameterName = "A";
        public const string FMapMethodOutputParameterName = "B";
        
        public const string OutMethodName = "Out";
       
        public const string OutClassName = "Out";
        public const string OutClassPredFieldName = "pred";
        
        public const string CataFunctionClassName = "Cata";
        public const string CataFunctionClassGenericParameterName = "T";
        public const string CataFunctionClassSeedFieldName = "seed";

        public const string AnaFunctionClassName = "Ana";
        public const string AnaFunctionClassGenericParameterName = "T";
        public const string AnaFunctionClassSeedFieldName = "seed";

        public const string InMethodName = "In";

        public const string OutFunctionClassName = "OutFunction";
        public const string InFunctionClassName = "InFunction";

        public const string FunctionSuffix = "Function";

        public const string AnaClassName = "AnaClass";
        public const string AnaClassGenericParameterName = "T";
        public const string AnaClassSeedFieldName = "seed";
        public const string AnaClassGeneratorFieldName = "generator";

        public const string InGeneratingFunctionClassName = "InGeneratingFunction";
    }
}
