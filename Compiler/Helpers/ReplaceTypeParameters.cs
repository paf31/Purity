using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;

namespace Purity.Compiler.Helpers
{
    public class ReplaceTypeParameters : ITypeVisitor<IType>, IFunctorVisitor<IFunctor>
    {
        private readonly IDictionary<string, IType> lookup;

        public ReplaceTypeParameters(IDictionary<string, IType> lookup)
        {
            this.lookup = lookup;
        }

        public static IType Replace(IType type, IDictionary<string, IType> lookup)
        {
            var visitor = new ReplaceTypeParameters(lookup);
            return type.AcceptVisitor(visitor);
        }

        public static IFunctor Replace(IFunctor functor, IDictionary<string, IType> lookup)
        {
            var visitor = new ReplaceTypeParameters(lookup);
            return functor.AcceptVisitor(visitor);
        }

        public IType VisitArrow(Compiler.Types.ArrowType t)
        {
            return new Types.ArrowType(Replace(t.Left, lookup), Replace(t.Right, lookup));
        }

        public IType VisitSynonym(Compiler.Types.TypeSynonym t)
        {
            return new Types.TypeSynonym(t.Identifier, t.TypeParameters.Select(p => Replace(p, lookup)).ToArray());
        }

        public IType VisitProduct(Compiler.Types.ProductType t)
        {
            return new Types.ProductType(Replace(t.Left, lookup), Replace(t.Right, lookup));
        }

        public IType VisitSum(Compiler.Types.SumType t)
        {
            return new Types.SumType(Replace(t.Left, lookup), Replace(t.Right, lookup));
        }

        public IType VisitParameter(Compiler.Types.TypeParameter t)
        {
            return lookup[t.Identifier];
        }

        public IFunctor VisitArrow(Compiler.Functors.ArrowFunctor f)
        {
            return new Functors.ArrowFunctor(Replace(f.Left, lookup), Replace(f.Right, lookup));
        }

        public IFunctor VisitConstant(Compiler.Functors.ConstantFunctor f)
        {
            return new Functors.ConstantFunctor(Replace(f.Value, lookup));
        }

        public IFunctor VisitSynonym(Compiler.Functors.FunctorSynonym f)
        {
            return new Functors.FunctorSynonym(f.Identifier, f.TypeParameters.Select(p => Replace(p, lookup)).ToArray());
        }

        public IFunctor VisitIdentity(Compiler.Functors.IdentityFunctor f)
        {
            return new Functors.IdentityFunctor();
        }

        public IFunctor VisitProduct(Compiler.Functors.ProductFunctor f)
        {
            return new Functors.ProductFunctor(Replace(f.Left, lookup), Replace(f.Right, lookup));
        }

        public IFunctor VisitSum(Compiler.Functors.SumFunctor f)
        {
            return new Functors.SumFunctor(Replace(f.Left, lookup), Replace(f.Right, lookup));
        }
    }
}
