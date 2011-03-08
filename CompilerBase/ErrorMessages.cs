using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Compiler
{
    public static class ErrorMessages
    {
        public const string ExpectedTypeSynonym = "Expected type synonym.";
        public const string ExpectedFixedPointType = "Expected fixed point type.";
        public const string ErrorInDeclaration = "Error in declaration '{0}': {1}";
        public const string UnableToParse = "Unable to parse input.";
        public const string UnableToResolveLFix = "Unable to resolve least fixed point type '{0}'.";
        public const string UnableToResolveGFix = "Unable to resolve greatest fixed point type '{0}'.";
        public const string UnableToResolveFix = "Unable to resolve fixed point type '{0}'.";
        public const string UnableToResolveType = "Unable to resolve type '{0}'.";
        public const string UnableToResolveFunctor = "Unable to resolve functor '{0}'.";
        public const string UnableToResolveData = "Unable to resolve data '{0}'.";
        public const string NameConflict = "Name conflict: '{0}'.";
        public const string UnableToInferFunctor = "Unable to infer functor.";
        public const string ExpectedArrowFunctor = "Expected arrow functor.";
        public const string ExpectedConstantFunctor = "Expected constant functor.";
        public const string ExpectedSumFunctor = "Expected sum functor.";
        public const string ExpectedProductFunctor = "Expected product functor.";
        public const string ExpectedIdentityFunctor = "Expected identity functor.";
        public const string ExpectedNamedFunctor = "Expected named functor.";
        public const string UnableToInferType = "Unable to infer type.";
        public const string ExpectedArrowType = "Expected arrow type.";
        public const string ExpectedSumType = "Expected sum type.";
        public const string ExpectedProductType = "Expected product type.";
        public const string ExpectedNamedType = "Expected named type.";
        public const string ExpectedLFixType = "Expected least fixed point type.";
        public const string ExpectedGFixType = "Expected greatest fixed point type.";
        public const string Expected = "Expected '{0}'.";
    }
}
