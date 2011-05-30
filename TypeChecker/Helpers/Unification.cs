using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Typechecker.Classes;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Typechecker.Utilities;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class Unification : IPartialTypeVisitor<IEnumerable<Constraint>>
    {
        private readonly IPartialType t2;

        public Unification(IPartialType t2)
        {
            this.t2 = t2;
        }

        public static IEnumerable<Constraint> Unify(IPartialType t1, IPartialType t2)
        {
            return t1.AcceptVisitor(new Unification(t2));
        }

        public IEnumerable<Constraint> VisitArrow(Types.ArrowType t1)
        {
            return t2.AcceptVisitor(new ArrowUnification(t1));
        }

        public IEnumerable<Constraint> VisitSynonym(Types.TypeSynonym t1)
        {
            return t2.AcceptVisitor(new SynonymUnification(t1));
        }

        public IEnumerable<Constraint> VisitProduct(Types.ProductType t1)
        {
            return t2.AcceptVisitor(new ProductUnification(t1));
        }

        public IEnumerable<Constraint> VisitSum(Types.SumType t1)
        {
            return t2.AcceptVisitor(new SumUnification(t1));
        }

        public IEnumerable<Constraint> VisitParameter(Types.TypeParameter t1)
        {
            return t2.AcceptVisitor(new ParameterUnification(t1));
        }

        public IEnumerable<Constraint> VisitUnknown(Types.UnknownType t1)
        {
            var unknown = t2 as Types.UnknownType;

            if (unknown != null)
            {
                if (unknown.Index == t1.Index)
                {
                    return Constraints.Identity();
                }
                else
                {
                    if (t1.Index < unknown.Index)
                    {
                        return Constraints.Single(t1.Index, unknown);
                    }
                    else
                    {
                        return Constraints.Single(unknown.Index, t1);
                    }
                }
            }
            else
            {
                OccursCheck.ThrowOnOccurs(t1.Index, t2);
                return Constraints.Single(t1.Index, t2);
            }
        }

        private class ArrowUnification : IPartialTypeVisitor<IEnumerable<Constraint>>
        {
            private readonly Types.ArrowType t1;

            public ArrowUnification(Types.ArrowType t1)
            {
                this.t1 = t1;
            }

            public IEnumerable<Constraint> VisitArrow(Types.ArrowType t)
            {
                var s1 = Unify(t.Left, t1.Left);
                var s2 = Unify(t.Right, t1.Right);
                return s1.Concat(s2);
            }

            public IEnumerable<Constraint> VisitSynonym(Types.TypeSynonym t)
            {
                throw new CompilerException("Expected arrow type, found " + t.Identifier);
            }

            public IEnumerable<Constraint> VisitProduct(Types.ProductType t)
            {
                throw new CompilerException("Expected arrow type, found product type.");
            }

            public IEnumerable<Constraint> VisitSum(Types.SumType t)
            {
                throw new CompilerException("Expected arrow type, found sum type.");
            }

            public IEnumerable<Constraint> VisitUnknown(Types.UnknownType t)
            {
                OccursCheck.ThrowOnOccurs(t.Index, t1);
                return Constraints.Single(t.Index, t1);
            }

            public IEnumerable<Constraint> VisitParameter(Types.TypeParameter t)
            {
                throw new CompilerException("Expected arrow type, found " + t.Identifier + ".");
            }
        }

        private class SynonymUnification : IPartialTypeVisitor<IEnumerable<Constraint>>
        {
            private readonly Types.TypeSynonym t1;

            public SynonymUnification(Types.TypeSynonym t1)
            {
                this.t1 = t1;
            }

            public IEnumerable<Constraint> VisitArrow(Types.ArrowType t)
            {
                throw new CompilerException("Expected " + t1.Identifier + " type, found arrow type.");
            }

            public IEnumerable<Constraint> VisitSynonym(Types.TypeSynonym t)
            {
                if (t.Identifier.Equals(t1.Identifier))
                {
                    return t.TypeParameters.Zip(t1.TypeParameters, Unify).Flatten();
                }
                else
                {
                    throw new CompilerException("Expected " + t.Identifier + " type, found " + t1.Identifier + ".");
                }
            }

            public IEnumerable<Constraint> VisitProduct(Types.ProductType t)
            {
                throw new CompilerException("Expected " + t1.Identifier + " type, found product type.");
            }

            public IEnumerable<Constraint> VisitSum(Types.SumType t)
            {
                throw new CompilerException("Expected " + t1.Identifier + ", found sum type.");
            }

            public IEnumerable<Constraint> VisitUnknown(Types.UnknownType t)
            {
                OccursCheck.ThrowOnOccurs(t.Index, t1);
                return Constraints.Single(t.Index, t1);
            }

            public IEnumerable<Constraint> VisitParameter(Types.TypeParameter t)
            {
                throw new CompilerException("Expected " + t1.Identifier + ", found " + t.Identifier + ".");
            }
        }

        private class ProductUnification : IPartialTypeVisitor<IEnumerable<Constraint>>
        {
            private readonly Types.ProductType t1;

            public ProductUnification(Types.ProductType t1)
            {
                this.t1 = t1;
            }

            public IEnumerable<Constraint> VisitArrow(Types.ArrowType t)
            {
                throw new CompilerException("Expected product type, found arrow type.");
            }

            public IEnumerable<Constraint> VisitSynonym(Types.TypeSynonym t)
            {
                throw new CompilerException("Expected product type, found " + t.Identifier + ".");
            }

            public IEnumerable<Constraint> VisitProduct(Types.ProductType t)
            {
                var s1 = Unify(t.Left, t1.Left);
                var s2 = Unify(t.Right, t1.Right);
                return s1.Concat(s2);
            }

            public IEnumerable<Constraint> VisitSum(Types.SumType t)
            {
                throw new CompilerException("Expected product type, found sum type.");
            }

            public IEnumerable<Constraint> VisitUnknown(Types.UnknownType t)
            {
                OccursCheck.ThrowOnOccurs(t.Index, t1);
                return Constraints.Single(t.Index, t1);
            }

            public IEnumerable<Constraint> VisitParameter(Types.TypeParameter t)
            {
                throw new CompilerException("Expected product type, found " + t.Identifier + ".");
            }
        }

        private class SumUnification : IPartialTypeVisitor<IEnumerable<Constraint>>
        {
            private readonly Types.SumType t1;

            public SumUnification(Types.SumType t1)
            {
                this.t1 = t1;
            }

            public IEnumerable<Constraint> VisitArrow(Types.ArrowType t)
            {
                throw new CompilerException("Expected sum type, found arrow type.");
            }

            public IEnumerable<Constraint> VisitSynonym(Types.TypeSynonym t)
            {
                throw new CompilerException("Expected sum type, found " + t.Identifier + ".");
            }

            public IEnumerable<Constraint> VisitProduct(Types.ProductType t)
            {
                throw new CompilerException("Expected sum type, found product type.");
            }

            public IEnumerable<Constraint> VisitSum(Types.SumType t)
            {
                var s1 = Unify(t.Left, t1.Left);
                var s2 = Unify(t.Right, t1.Right);
                return s1.Concat(s2);
            }

            public IEnumerable<Constraint> VisitUnknown(Types.UnknownType t)
            {
                OccursCheck.ThrowOnOccurs(t.Index, t1);
                return Constraints.Single(t.Index, t1);
            }

            public IEnumerable<Constraint> VisitParameter(Types.TypeParameter t)
            {
                throw new CompilerException("Expected sum type, found " + t.Identifier + ".");
            }
        }

        private class ParameterUnification : IPartialTypeVisitor<IEnumerable<Constraint>>
        {
            private readonly Types.TypeParameter t1;

            public ParameterUnification(Types.TypeParameter t1)
            {
                this.t1 = t1;
            }

            public IEnumerable<Constraint> VisitArrow(Types.ArrowType t)
            {
                throw new CompilerException("Expected " + t1.Identifier + ", found arrow type.");
            }

            public IEnumerable<Constraint> VisitSynonym(Types.TypeSynonym t)
            {
                throw new CompilerException("Expected " + t1.Identifier + ", found " + t.Identifier + ".");
            }

            public IEnumerable<Constraint> VisitProduct(Types.ProductType t)
            {
                throw new CompilerException("Expected " + t1.Identifier + ", found product type.");
            }

            public IEnumerable<Constraint> VisitSum(Types.SumType t)
            {
                throw new CompilerException("Expected " + t1.Identifier + ", found sum type.");
            }

            public IEnumerable<Constraint> VisitUnknown(Types.UnknownType t)
            {
                OccursCheck.ThrowOnOccurs(t.Index, t1);
                return Constraints.Single(t.Index, t1);
            }

            public IEnumerable<Constraint> VisitParameter(Types.TypeParameter t)
            {
                if (t.Identifier.Equals(t1.Identifier))
                {
                    return Constraints.Identity();
                }
                else
                {
                    throw new CompilerException("Expected " + t1.Identifier + ", found " + t.Identifier + ".");
                }
            }
        }
    }
}
