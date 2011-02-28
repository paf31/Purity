using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class FunctorConverter : IPartialFunctorVisitor
    {
        public IFunctor Result
        {
            get;
            set;
        }

        public IFunctor Convert(IPartialFunctor functor)
        {
            functor.AcceptVisitor(this);
            return Result;
        }

        public void VisitArrow(Functors.ArrowFunctor f)
        {
            Result = new Purity.Compiler.Functors.ArrowFunctor(new TypeConverter().Convert(f.Left), Convert(f.Right));
        }

        public void VisitConstant(Functors.ConstantFunctor f)
        {
            Result = new Purity.Compiler.Functors.ConstantFunctor(new TypeConverter().Convert(f.Value));
        }

        public void VisitIdentity(Functors.IdentityFunctor f)
        {
            Result = new Purity.Compiler.Functors.IdentityFunctor();
        }

        public void VisitProduct(Functors.ProductFunctor f)
        {
            Result = new Purity.Compiler.Functors.ProductFunctor(Convert(f.Left), Convert(f.Right));
        }

        public void VisitSum(Functors.SumFunctor f)
        {
            Result = new Purity.Compiler.Functors.SumFunctor(Convert(f.Left), Convert(f.Right));
        }

        public void VisitUnknown(Functors.UnknownFunctor unknownType)
        {
            throw new CompilerException("Unable to infer functor type.");
        }
    }
}