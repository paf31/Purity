using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class ReplaceTypeParameters : ITypeVisitor<IPartialType>, IFunctorVisitor<IPartialFunctor>
    {
        private readonly IDictionary<string, int> lookup;

        public ReplaceTypeParameters(IDictionary<string, int> lookup)
        {
            this.lookup = lookup;
        }

        public static IPartialType Replace(IType type, IDictionary<string, int> lookup)
        {
            var visitor = new ReplaceTypeParameters(lookup);
            return type.AcceptVisitor(visitor);
        }

        public static IPartialFunctor Replace(IFunctor functor, IDictionary<string, int> lookup)
        {
            var visitor = new ReplaceTypeParameters(lookup);
            return functor.AcceptVisitor(visitor);
        }

        public IPartialType VisitArrow(Compiler.Types.ArrowType t)
        {
            return new Types.ArrowType(Replace(t.Left, lookup), Replace(t.Right, lookup));
        }

        public IPartialType VisitSynonym(Compiler.Types.TypeSynonym t)
        {
            return new Types.TypeSynonym(t.Identifier);
        }

        public IPartialType VisitProduct(Compiler.Types.ProductType t)
        {
            return new Types.ProductType(Replace(t.Left, lookup), Replace(t.Right, lookup));
        }

        public IPartialType VisitSum(Compiler.Types.SumType t)
        {
            return new Types.SumType(Replace(t.Left, lookup), Replace(t.Right, lookup));
        }

        public IPartialType VisitLFix(Compiler.Types.LFixType t)
        {
            var result = new Types.LFixType(Replace(t.Functor, lookup));
            result.Identifier = t.Identifier;
            return result;
        }

        public IPartialType VisitGFix(Compiler.Types.GFixType t)
        {
            var result = new Types.GFixType(Replace(t.Functor, lookup));
            result.Identifier = t.Identifier;
            return result;
        }

        public IPartialType VisitParameter(Compiler.Types.TypeParameter t)
        {
            return new Types.UnknownType(lookup[t.Identifier]);
        }

        public IPartialFunctor VisitArrow(Compiler.Functors.ArrowFunctor f)
        {
            return new Functors.ArrowFunctor(Replace(f.Left, lookup), Replace(f.Right, lookup));
        }

        public IPartialFunctor VisitConstant(Compiler.Functors.ConstantFunctor f)
        {
            return new Functors.ConstantFunctor(Replace(f.Value, lookup));
        }

        public IPartialFunctor VisitSynonym(Compiler.Functors.FunctorSynonym f)
        {
            return new Functors.FunctorSynonym(f.Identifier);
        }

        public IPartialFunctor VisitIdentity(Compiler.Functors.IdentityFunctor f)
        {
            return new Functors.IdentityFunctor();
        }

        public IPartialFunctor VisitProduct(Compiler.Functors.ProductFunctor f)
        {
            return new Functors.ProductFunctor(Replace(f.Left, lookup), Replace(f.Right, lookup));
        }

        public IPartialFunctor VisitSum(Compiler.Functors.SumFunctor f)
        {
            return new Functors.SumFunctor(Replace(f.Left, lookup), Replace(f.Right, lookup));
        }
    }
}
