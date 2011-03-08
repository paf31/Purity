using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Typechecker.Types;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Typechecker.Functors;
using Purity.Compiler.Typechecker.Constraints;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class ConstrainedDataCreator : IDataVisitor
    {
        public int TypeIndex
        {
            get;
            set;
        }

        public int FunctorIndex
        {
            get;
            set;
        }

        public IConstrainedData Result
        {
            get;
            set;
        }

        public int RootIndex
        {
            get;
            set;
        }

        public IList<IConstraint> Constraints
        {
            get;
            set;
        }

        public IDictionary<int, IType> KnownTypes
        {
            get;
            set;
        }

        public IDictionary<int, Tuple<int, int>> KnownFunctorApplications
        {
            get;
            set;
        }

        public ConstrainedDataCreator()
        {
            Constraints = new List<IConstraint>();
            KnownTypes = new Dictionary<int, IType>();
            KnownFunctorApplications = new Dictionary<int, Tuple<int, int>>();
        }

        public IConstrainedData Convert(IData data, out int index)
        {
            data.AcceptVisitor(this);
            index = RootIndex;
            return Result;
        }

        public void VisitAna(Compiler.Data.Ana d)
        {
            int coalgebra;
            int carrier = TypeIndex++;
            int fCarrier = TypeIndex++;
            int gfix = TypeIndex++;
            int ana = TypeIndex++;
            int boxed = TypeIndex++;
            int functor = FunctorIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Ana(Convert(d.Coalgebra, out coalgebra));
            result.CarrierType = new UnknownType(carrier);
            result.Functor = new UnknownFunctor(functor);
            result.GFixType = new UnknownType(gfix);
            Result = result;

            Constraints.Add(new GFixConstraint(functor, gfix));
            Constraints.Add(new ArrowConstraint(carrier, fCarrier, coalgebra));
            Constraints.Add(new ArrowConstraint(carrier, boxed, ana));
            Constraints.Add(new SynonymConstraint(gfix, boxed));

            KnownFunctorApplications[fCarrier] = Tuple.Create(functor, carrier);

            RootIndex = ana;
        }

        public void VisitApplication(Compiler.Data.Application d)
        {
            int left;
            int right;
            int output = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Application(Convert(d.Left, out left), Convert(d.Right, out right));
            result.LeftType = new UnknownType(left);
            result.RightType = new UnknownType(right);
            Result = result;

            Constraints.Add(new ArrowConstraint(right, output, left));

            RootIndex = output;
        }

        public void VisitCase(Compiler.Data.Case d)
        {
            int left;
            int right;

            int t1 = TypeIndex++;
            int t2 = TypeIndex++;
            int sum = TypeIndex++;

            int arrow = TypeIndex++;
            int output = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Case(Convert(d.Left, out left), Convert(d.Right, out right));
            result.LeftType = new UnknownType(t1);
            result.RightType = new UnknownType(t2);
            result.ResultType = new UnknownType(output);
            Result = result;

            Constraints.Add(new SumConstraint(t1, t2, sum));
            Constraints.Add(new ArrowConstraint(t1, output, left));
            Constraints.Add(new ArrowConstraint(t2, output, right));
            Constraints.Add(new ArrowConstraint(sum, output, arrow));

            RootIndex = arrow;
        }

        public void VisitCata(Compiler.Data.Cata d)
        {
            int algebra;
            int carrier = TypeIndex++;
            int fCarrier = TypeIndex++;
            int lfix = TypeIndex++;
            int cata = TypeIndex++;
            int boxed = TypeIndex++;
            int functor = FunctorIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Cata(Convert(d.Algebra, out algebra));
            result.CarrierType = new UnknownType(carrier);
            result.Functor = new UnknownFunctor(functor);
            result.LFixType = new UnknownType(lfix);
            Result = result;

            Constraints.Add(new LFixConstraint(functor, lfix));
            Constraints.Add(new ArrowConstraint(fCarrier, carrier, algebra));
            Constraints.Add(new ArrowConstraint(boxed, carrier, cata));
            Constraints.Add(new SynonymConstraint(lfix, boxed));

            KnownFunctorApplications[fCarrier] = Tuple.Create(functor, carrier);

            RootIndex = cata;
        }

        public void VisitComposition(Compiler.Data.Composition d)
        {
            int left;
            int right;
            int composition = TypeIndex++;
            int t1 = TypeIndex++;
            int t2 = TypeIndex++;
            int t3 = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Composition(Convert(d.Left, out left), Convert(d.Right, out right));
            result.LeftType = new UnknownType(t1);
            result.MiddleType = new UnknownType(t2);
            result.RightType = new UnknownType(t3);
            Result = result;

            Constraints.Add(new ArrowConstraint(t2, t3, left));
            Constraints.Add(new ArrowConstraint(t1, t2, right));
            Constraints.Add(new ArrowConstraint(t1, t3, composition));

            RootIndex = composition;
        }

        public void VisitConst(Compiler.Data.Const d)
        {
            int value;
            int input = TypeIndex++;
            int arrow = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Const(Convert(d.Value, out value));
            result.InputType = new UnknownType(input);
            result.OutputType = new UnknownType(value);
            Result = result;

            Constraints.Add(new ArrowConstraint(input, value, arrow));

            RootIndex = arrow;
        }

        public void VisitSynonym(Compiler.Data.DataSynonym d)
        {
            int index = TypeIndex++;
            KnownTypes[index] = Container.ResolveValue(d.Identifier).Type;
            Result = new Purity.Compiler.Typechecker.Data.DataSynonym(d.Identifier);
            RootIndex = index;
        }

        public void VisitIdentity(Compiler.Data.Identity d)
        {
            int t = TypeIndex++;
            int arrow = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Identity();
            result.Type = new UnknownType(t);
            Result = result;

            Constraints.Add(new ArrowConstraint(t, t, arrow));

            RootIndex = arrow;
        }

        public void VisitInl(Compiler.Data.Inl d)
        {
            int t1 = TypeIndex++;
            int t2 = TypeIndex++;
            int sum = TypeIndex++;
            int arrow = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Inl();
            result.LeftType = new UnknownType(t1);
            result.RightType = new UnknownType(t2);
            Result = result;

            Constraints.Add(new SumConstraint(t1, t2, sum));
            Constraints.Add(new ArrowConstraint(t1, sum, arrow));

            RootIndex = arrow;
        }

        public void VisitInr(Compiler.Data.Inr d)
        {
            int t1 = TypeIndex++;
            int t2 = TypeIndex++;
            int sum = TypeIndex++;
            int arrow = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Inr();
            result.LeftType = new UnknownType(t1);
            result.RightType = new UnknownType(t2);
            Result = result;

            Constraints.Add(new SumConstraint(t1, t2, sum));
            Constraints.Add(new ArrowConstraint(t2, sum, arrow));

            RootIndex = arrow;
        }

        public void VisitOutl(Compiler.Data.Outl d)
        {
            int t1 = TypeIndex++;
            int t2 = TypeIndex++;
            int product = TypeIndex++;
            int arrow = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Outl();
            result.LeftType = new UnknownType(t1);
            result.RightType = new UnknownType(t2);
            Result = result;

            Constraints.Add(new ProductConstraint(t1, t2, product));
            Constraints.Add(new ArrowConstraint(product, t1, arrow));

            RootIndex = arrow;
        }

        public void VisitOutr(Compiler.Data.Outr d)
        {
            int t1 = TypeIndex++;
            int t2 = TypeIndex++;
            int product = TypeIndex++;
            int arrow = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Outr();
            result.LeftType = new UnknownType(t1);
            result.RightType = new UnknownType(t2);
            Result = result;

            Constraints.Add(new ProductConstraint(t1, t2, product));
            Constraints.Add(new ArrowConstraint(product, t2, arrow));

            RootIndex = arrow;
        }

        public void VisitSplit(Compiler.Data.Split d)
        {
            int left;
            int right;
            int t1 = TypeIndex++;
            int t2 = TypeIndex++;
            int input = TypeIndex++;
            int product = TypeIndex++;
            int arrow = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Split(Convert(d.Left, out left), Convert(d.Right, out right));
            result.LeftType = new UnknownType(t1);
            result.RightType = new UnknownType(t2);
            result.InputType = new UnknownType(input);
            Result = result;

            Constraints.Add(new ProductConstraint(t1, t2, product));
            Constraints.Add(new ArrowConstraint(input, product, arrow));
            Constraints.Add(new ArrowConstraint(input, t1, left));
            Constraints.Add(new ArrowConstraint(input, t2, right));

            RootIndex = arrow;
        }

        public void VisitIn(Compiler.Data.In d)
        {
            int functor = FunctorIndex++;
            int fix = TypeIndex++;
            int ffix = TypeIndex++;
            int arrow = TypeIndex++;
            int boxed = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.In();
            result.Source = new UnknownType(fix);
            result.Functor = new UnknownFunctor(functor);
            Result = result;

            Constraints.Add(new FixConstraint(functor, fix));
            Constraints.Add(new ArrowConstraint(boxed, ffix, arrow));
            Constraints.Add(new SynonymConstraint(fix, boxed));

            KnownFunctorApplications[ffix] = Tuple.Create(functor, boxed);

            RootIndex = arrow;
        }

        public void VisitOut(Compiler.Data.Out d)
        {
            int functor = FunctorIndex++;
            int fix = TypeIndex++;
            int ffix = TypeIndex++;
            int arrow = TypeIndex++;
            int boxed = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Out();
            result.Target = new UnknownType(fix);
            result.Functor = new UnknownFunctor(functor);
            Result = result;

            Constraints.Add(new FixConstraint(functor, fix));
            Constraints.Add(new ArrowConstraint(ffix, boxed, arrow));
            Constraints.Add(new SynonymConstraint(fix, boxed));

            KnownFunctorApplications[ffix] = Tuple.Create(functor, boxed);

            RootIndex = arrow;
        }

        public void VisitCurry(Compiler.Data.Curried curried)
        {
            int uncurried;
            int first = TypeIndex++;
            int second = TypeIndex++;
            int product = TypeIndex++;
            int innerArrow = TypeIndex++;
            int arrow = TypeIndex++;
            int output = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Curried(Convert(curried.Function, out uncurried));
            result.First = new UnknownType(first);
            result.Second = new UnknownType(second);
            result.Output = new UnknownType(output);
            Result = result;

            Constraints.Add(new ProductConstraint(first, second, product));
            Constraints.Add(new ArrowConstraint(second, output, innerArrow));
            Constraints.Add(new ArrowConstraint(first, innerArrow, arrow));
            Constraints.Add(new ArrowConstraint(product, output, uncurried));

            RootIndex = arrow;
        }

        public void VisitUncurry(Compiler.Data.Uncurried uncurried)
        {
            int curried;
            int first = TypeIndex++;
            int second = TypeIndex++;
            int product = TypeIndex++;
            int innerArrow = TypeIndex++;
            int arrow = TypeIndex++;
            int output = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Uncurried(Convert(uncurried.Function, out curried));
            result.First = new UnknownType(first);
            result.Second = new UnknownType(second);
            result.Output = new UnknownType(output);
            Result = result;

            Constraints.Add(new ProductConstraint(first, second, product));
            Constraints.Add(new ArrowConstraint(second, output, innerArrow));
            Constraints.Add(new ArrowConstraint(first, innerArrow, curried));
            Constraints.Add(new ArrowConstraint(product, output, arrow));

            RootIndex = arrow;
        }

        public void VisitCl(Compiler.Data.Cl cl)
        {
            int function;
            int a = TypeIndex++;
            int b = TypeIndex++;
            int c = TypeIndex++;
            int f1 = TypeIndex++;
            int f2 = TypeIndex++;
            int arrow = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Cl(Convert(cl.Function, out function));
            result.A = new UnknownType(a);
            result.B = new UnknownType(b);
            result.C = new UnknownType(c);
            Result = result;

            Constraints.Add(new ArrowConstraint(a, b, function));
            Constraints.Add(new ArrowConstraint(b, c, f1));
            Constraints.Add(new ArrowConstraint(a, c, f2));
            Constraints.Add(new ArrowConstraint(f1, f2, arrow));

            RootIndex = arrow;
        }

        public void VisitCr(Compiler.Data.Cr cr)
        {
            int function;
            int a = TypeIndex++;
            int b = TypeIndex++;
            int c = TypeIndex++;
            int f1 = TypeIndex++;
            int f2 = TypeIndex++;
            int arrow = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Cr(Convert(cr.Function, out function));
            result.A = new UnknownType(a);
            result.B = new UnknownType(b);
            result.C = new UnknownType(c);
            Result = result;

            Constraints.Add(new ArrowConstraint(b, c, function));
            Constraints.Add(new ArrowConstraint(a, b, f1));
            Constraints.Add(new ArrowConstraint(a, c, f2));
            Constraints.Add(new ArrowConstraint(f1, f2, arrow));

            RootIndex = arrow;
        }

        public void VisitBox(Compiler.Data.Box d)
        {
            int target = TypeIndex++;
            int boxed = TypeIndex++;
            int arrow = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Box();
            result.Type = new UnknownType(boxed);
            result.Target = new UnknownType(target);
            Result = result;

            Constraints.Add(new SynonymConstraint(target, boxed));
            Constraints.Add(new ArrowConstraint(target, boxed, arrow));

            RootIndex = arrow;
        }

        public void VisitUnbox(Compiler.Data.Unbox d)
        {
            int target = TypeIndex++;
            int boxed = TypeIndex++;
            int arrow = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Unbox();
            result.Type = new UnknownType(boxed);
            result.Target = new UnknownType(target);
            Result = result;

            Constraints.Add(new SynonymConstraint(target, boxed));
            Constraints.Add(new ArrowConstraint(boxed, target, arrow));

            RootIndex = arrow;
        }
    }
}
