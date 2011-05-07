using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using System.Reflection.Emit;
using Purity.Core;
using Purity.Core.Functions;

namespace Purity.Compiler.Helpers
{
    public class FmapCompiler : ITypeVisitor
    {
        private readonly ILGenerator body;
        private readonly string variableName;
        private readonly Type[] genericParameters;
        private readonly IRuntimeContainer runtimeContainer;

        public FmapCompiler(ILGenerator body, string variableName, Type[] genericParameters, IRuntimeContainer runtimeContainer)
        {
            this.body = body;
            this.variableName = variableName;
            this.genericParameters = genericParameters;
            this.runtimeContainer = runtimeContainer;
        }

        private void EmitConstant(IType type)
        {
            var genericParameter = new TypeConverter(runtimeContainer, genericParameters.Skip(2).ToArray()).Convert(type);
            body.Emit(OpCodes.Newobj, TypeBuilder.GetConstructor(
                typeof(IdentityFunction<>).MakeGenericType(genericParameter),
                typeof(IdentityFunction<>).GetConstructors()[0]));
        }

        public void VisitArrow(Types.ArrowType t)
        {
            t.Right.AcceptVisitor(this);

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(ArrowFunctorAction<,,>).MakeGenericType(
                    new TypeConverter(runtimeContainer, genericParameters.Skip(2).ToArray()).Convert(t.Left),
                    FunctorTypeMapper.Map(t.Right, variableName, genericParameters[0], genericParameters, runtimeContainer),
                    FunctorTypeMapper.Map(t.Right, variableName, genericParameters[1], genericParameters, runtimeContainer)),
                typeof(ArrowFunctorAction<,,>).GetConstructors()[0]));
        }

        public void VisitSynonym(Types.TypeSynonym t)
        {
            // TODO: throw if the type parameters mention the variable name

            EmitConstant(t);
        }

        public void VisitProduct(Types.ProductType t)
        {
            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(OutlFunction<,>).MakeGenericType(
                    FunctorTypeMapper.Map(t.Left, variableName, genericParameters[0], genericParameters, runtimeContainer),
                    FunctorTypeMapper.Map(t.Right, variableName, genericParameters[0], genericParameters, runtimeContainer)),
                typeof(OutlFunction<,>).GetConstructors()[0]));

            t.Left.AcceptVisitor(this);

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(CompositeFunction<,,>).MakeGenericType(
                    FunctorTypeMapper.Map(t, variableName, genericParameters[0], genericParameters, runtimeContainer),
                    FunctorTypeMapper.Map(t.Left, variableName, genericParameters[0], genericParameters, runtimeContainer),
                    FunctorTypeMapper.Map(t.Left, variableName, genericParameters[1], genericParameters, runtimeContainer)),
                typeof(CompositeFunction<,,>).GetConstructors()[0]));

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(OutrFunction<,>).MakeGenericType(
                    FunctorTypeMapper.Map(t.Left, variableName, genericParameters[0], genericParameters, runtimeContainer),
                    FunctorTypeMapper.Map(t.Right, variableName, genericParameters[0], genericParameters, runtimeContainer)),
                typeof(OutrFunction<,>).GetConstructors()[0]));

            t.Right.AcceptVisitor(this);

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(CompositeFunction<,,>).MakeGenericType(
                    FunctorTypeMapper.Map(t, variableName, genericParameters[0], genericParameters, runtimeContainer),
                    FunctorTypeMapper.Map(t.Right, variableName, genericParameters[0], genericParameters, runtimeContainer),
                    FunctorTypeMapper.Map(t.Right, variableName, genericParameters[1], genericParameters, runtimeContainer)),
                typeof(CompositeFunction<,,>).GetConstructors()[0]));

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(SplitFunction<,,>).MakeGenericType(
                    FunctorTypeMapper.Map(t, variableName, genericParameters[0], genericParameters, runtimeContainer),
                    FunctorTypeMapper.Map(t.Left, variableName, genericParameters[1], genericParameters, runtimeContainer),
                    FunctorTypeMapper.Map(t.Right, variableName, genericParameters[1], genericParameters, runtimeContainer)),
                typeof(SplitFunction<,,>).GetConstructors()[0]));
        }

        public void VisitSum(Types.SumType t)
        {
            t.Left.AcceptVisitor(this);

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(InlFunction<,>).MakeGenericType(
                    FunctorTypeMapper.Map(t.Left, variableName, genericParameters[1], genericParameters, runtimeContainer),
                    FunctorTypeMapper.Map(t.Right, variableName, genericParameters[1], genericParameters, runtimeContainer)),
                typeof(InlFunction<,>).GetConstructors()[0]));

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(CompositeFunction<,,>).MakeGenericType(
                    FunctorTypeMapper.Map(t.Left, variableName, genericParameters[0], genericParameters, runtimeContainer),
                    FunctorTypeMapper.Map(t.Left, variableName, genericParameters[1], genericParameters, runtimeContainer),
                    FunctorTypeMapper.Map(t, variableName, genericParameters[1], genericParameters, runtimeContainer)),
                typeof(CompositeFunction<,,>).GetConstructors()[0]));

            t.Right.AcceptVisitor(this);

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(InrFunction<,>).MakeGenericType(
                    FunctorTypeMapper.Map(t.Left, variableName, genericParameters[1], genericParameters, runtimeContainer),
                    FunctorTypeMapper.Map(t.Right, variableName, genericParameters[1], genericParameters, runtimeContainer)),
                typeof(InrFunction<,>).GetConstructors()[0]));

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(CompositeFunction<,,>).MakeGenericType(
                    FunctorTypeMapper.Map(t.Right, variableName, genericParameters[0], genericParameters, runtimeContainer),
                    FunctorTypeMapper.Map(t.Right, variableName, genericParameters[1], genericParameters, runtimeContainer),
                    FunctorTypeMapper.Map(t, variableName, genericParameters[1], genericParameters, runtimeContainer)),
                typeof(CompositeFunction<,,>).GetConstructors()[0]));

            body.Emit(OpCodes.Newobj,
                TypeBuilder.GetConstructor(
                typeof(CaseFunction<,,>).MakeGenericType(
                    FunctorTypeMapper.Map(t.Left, variableName, genericParameters[0], genericParameters, runtimeContainer),
                    FunctorTypeMapper.Map(t.Right, variableName, genericParameters[0], genericParameters, runtimeContainer),
                    FunctorTypeMapper.Map(t, variableName, genericParameters[1], genericParameters, runtimeContainer)),
                typeof(CaseFunction<,,>).GetConstructors()[0]));
        }

        public void VisitParameter(Types.TypeParameter t)
        {
            if (t.Identifier.Equals(variableName))
            {
                body.Emit(OpCodes.Ldarg_0);
            }
            else 
            {
                EmitConstant(t);
            }
        }
    }
}
