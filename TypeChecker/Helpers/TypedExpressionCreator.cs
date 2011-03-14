﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class TypedExpressionCreator : IConstrainedDataVisitor
    {
        public ITypedExpression Result
        {
            get;
            set;
        }

        public ITypedExpression Convert(IConstrainedData data)
        {
            data.AcceptVisitor(this);
            return Result;
        }

        public void VisitAna(Data.Ana d)
        {
            Result = new Purity.Compiler.TypedExpressions.Ana(
                new TypeConverter().Convert(d.CarrierType),
                new TypeConverter().Convert(d.GFixType));
        }

        public void VisitApplication(Data.Application d)
        {
            Result = new Purity.Compiler.TypedExpressions.Application(
                Convert(d.Left),
                Convert(d.Right),
                new TypeConverter().Convert(d.LeftType),
                new TypeConverter().Convert(d.RightType));
        }

        public void VisitCase(Data.Case d)
        {
            Result = new Purity.Compiler.TypedExpressions.Case(
                Convert(d.Left),
                Convert(d.Right),
                new TypeConverter().Convert(d.LeftType),
                new TypeConverter().Convert(d.RightType),
                new TypeConverter().Convert(d.ResultType));
        }

        public void VisitCata(Data.Cata d)
        {
            Result = new Purity.Compiler.TypedExpressions.Cata(
                new TypeConverter().Convert(d.CarrierType),
                new TypeConverter().Convert(d.LFixType));
        }

        public void VisitComposition(Data.Composition d)
        {
            Result = new Purity.Compiler.TypedExpressions.Composition(
                Convert(d.Left),
                Convert(d.Right),
                new TypeConverter().Convert(d.LeftType),
                new TypeConverter().Convert(d.MiddleType),
                new TypeConverter().Convert(d.RightType));
        }

        public void VisitConst(Data.Const d)
        {
            Result = new Purity.Compiler.TypedExpressions.Const(
                Convert(d.Value),
                new TypeConverter().Convert(d.InputType),
                new TypeConverter().Convert(d.OutputType));
        }

        public void VisitIdentity(Data.Identity d)
        {
            Result = new Purity.Compiler.TypedExpressions.Identity(
                new TypeConverter().Convert(d.Type));
        }

        public void VisitInl(Data.Inl d)
        {
            Result = new Purity.Compiler.TypedExpressions.Inl(
                new TypeConverter().Convert(d.LeftType),
                new TypeConverter().Convert(d.RightType));
        }

        public void VisitInr(Data.Inr d)
        {
            Result = new Purity.Compiler.TypedExpressions.Inr(
                new TypeConverter().Convert(d.LeftType),
                new TypeConverter().Convert(d.RightType));
        }

        public void VisitOutl(Data.Outl d)
        {
            Result = new Purity.Compiler.TypedExpressions.Outl(
                new TypeConverter().Convert(d.LeftType),
                new TypeConverter().Convert(d.RightType));
        }

        public void VisitOutr(Data.Outr d)
        {
            Result = new Purity.Compiler.TypedExpressions.Outr(
                new TypeConverter().Convert(d.LeftType),
                new TypeConverter().Convert(d.RightType));
        }

        public void VisitSplit(Data.Split d)
        {
            Result = new Purity.Compiler.TypedExpressions.Split(
                Convert(d.Left),
                Convert(d.Right),
                new TypeConverter().Convert(d.LeftType),
                new TypeConverter().Convert(d.RightType),
                new TypeConverter().Convert(d.InputType));
        }

        public void VisitIn(Data.In d)
        {
            Result = new Purity.Compiler.TypedExpressions.In(
                new TypeConverter().Convert(d.Source));
        }

        public void VisitOut(Data.Out d)
        {
            Result = new Purity.Compiler.TypedExpressions.Out(
                new TypeConverter().Convert(d.Target));
        }

        public void VisitCurry(Data.Curried d)
        {
            Result = new Purity.Compiler.TypedExpressions.Curried(
                Convert(d.Function),
                new TypeConverter().Convert(d.First),
                new TypeConverter().Convert(d.Second),
                new TypeConverter().Convert(d.Output));
        }

        public void VisitUncurry(Data.Uncurried d)
        {
            Result = new Purity.Compiler.TypedExpressions.Uncurried(
                Convert(d.Function),
                new TypeConverter().Convert(d.First),
                new TypeConverter().Convert(d.Second),
                new TypeConverter().Convert(d.Output));
        }

        public void VisitSynonym(Data.DataSynonym d)
        {
            Result = new Purity.Compiler.TypedExpressions.DataSynonym(d.Identifier);
        }

        public void VisitBox(Data.Box d)
        {
            Result = new Purity.Compiler.TypedExpressions.Box(
                new TypeConverter().Convert(d.Target), 
                new TypeConverter().Convert(d.Type));
        }

        public void VisitUnbox(Data.Unbox d)
        {
            Result = new Purity.Compiler.TypedExpressions.Unbox(
                new TypeConverter().Convert(d.Target), 
                new TypeConverter().Convert(d.Type));
        }

        public void VisitAbstraction(Data.Abstraction d)
        {
            Result = new Purity.Compiler.TypedExpressions.Abstraction(
                d.Variable,
                Convert(d.Body), 
                new TypeConverter().Convert(d.BodyType),
                new TypeConverter().Convert(d.VariableType));
        }

        public void VisitVariable(Data.Variable d)
        {
            Result = new Purity.Compiler.TypedExpressions.Variable(
                d.Name,
                new TypeConverter().Convert(d.Type));
        }
    }
}
