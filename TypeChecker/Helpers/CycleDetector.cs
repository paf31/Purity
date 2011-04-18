using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Exceptions;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class CycleDetector : IConstraintVisitor
    {
        private readonly IEnumerable<IConstraint> constraints;
        private readonly Stack<int> visitedIndices = new Stack<int>();

        public CycleDetector(IEnumerable<IConstraint> constraints)
        {
            this.constraints = constraints;
        }

        public void ThrowOnCycleDetected(int index)
        {
            if (visitedIndices.Contains(index))
            {
                throw new CompilerException(ErrorMessages.CycleDetected);
            }

            visitedIndices.Push(index);

            foreach (var constraint in constraints)
            {
                if (constraint.Index == index)
                {
                    constraint.AcceptVisitor(this);
                }
            }

            visitedIndices.Pop();
        }

        public static void ThrowOnCycleDetected(IEnumerable<IConstraint> constraints)
        {
            foreach (var constraint in constraints)
            {
                var visitor = new CycleDetector(constraints);
                visitor.ThrowOnCycleDetected(constraint.Index);
            }
        }

        public void VisitArrow(Constraints.ArrowConstraint c)
        {
            ThrowOnCycleDetected(c.Left);
            ThrowOnCycleDetected(c.Right);
        }

        public void VisitProduct(Constraints.ProductConstraint c)
        {
            ThrowOnCycleDetected(c.Left);
            ThrowOnCycleDetected(c.Right);
        }

        public void VisitSum(Constraints.SumConstraint c)
        {
            ThrowOnCycleDetected(c.Left);
            ThrowOnCycleDetected(c.Right);
        }
    }
}
