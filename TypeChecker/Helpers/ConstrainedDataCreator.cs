using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purity.Compiler.Interfaces;
using Purity.Compiler.Exceptions;
using Purity.Compiler.Typechecker.Interfaces;
using Purity.Compiler.Typechecker.Types;
using Purity.Compiler.Typechecker.Constraints;

namespace Purity.Compiler.Typechecker.Helpers
{
    public class ConstrainedDataCreator : IDataVisitor<Tuple<IConstrainedData, int>>
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

        public IList<IConstraint> Constraints
        {
            get;
            set;
        }

        public IDictionary<int, IPartialType> KnownTypes
        {
            get;
            set;
        }

        public IDictionary<string, int> Variables
        {
            get;
            set;
        }

        public ConstrainedDataCreator()
        {
            Constraints = new List<IConstraint>();
            KnownTypes = new Dictionary<int, IPartialType>();
            Variables = new Dictionary<string, int>();
        }

        public IConstrainedData Convert(IData data, out int index)
        {
            var pair = data.AcceptVisitor(this);
            index = pair.Item2;
            return pair.Item1;
        }

        public Tuple<IConstrainedData, int> VisitApplication(Compiler.Data.Application d)
        {
            int left;
            int right;
            int output = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Application(Convert(d.Left, out left), Convert(d.Right, out right));
            result.LeftType = new UnknownType(left);
            result.RightType = new UnknownType(right);

            Constraints.Add(new ArrowConstraint(right, output, left));

            return Tuple.Create<IConstrainedData, int>(result, output);
        }

        public Tuple<IConstrainedData, int> VisitCase(Compiler.Data.Case d)
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

            Constraints.Add(new SumConstraint(t1, t2, sum));
            Constraints.Add(new ArrowConstraint(t1, output, left));
            Constraints.Add(new ArrowConstraint(t2, output, right));
            Constraints.Add(new ArrowConstraint(sum, output, arrow));

            return Tuple.Create<IConstrainedData, int>(result, arrow);
        }

        public Tuple<IConstrainedData, int> VisitComposition(Compiler.Data.Composition d)
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

            Constraints.Add(new ArrowConstraint(t2, t3, left));
            Constraints.Add(new ArrowConstraint(t1, t2, right));
            Constraints.Add(new ArrowConstraint(t1, t3, composition));

            return Tuple.Create<IConstrainedData, int>(result, composition);
        }

        public Tuple<IConstrainedData, int> VisitConst(Compiler.Data.Const d)
        {
            int value;
            int input = TypeIndex++;
            int arrow = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Const(Convert(d.Value, out value));
            result.InputType = new UnknownType(input);
            result.OutputType = new UnknownType(value);

            Constraints.Add(new ArrowConstraint(input, value, arrow));

            return Tuple.Create<IConstrainedData, int>(result, arrow);
        }

        public Tuple<IConstrainedData, int> VisitSynonym(Compiler.Data.DataSynonym d)
        {
            var declaration = Container.ResolveValue(d.Identifier);

            var lookup = new Dictionary<string, int>();
            
            var result = new Purity.Compiler.Typechecker.Data.DataSynonym(d.Identifier);

            if (declaration.TypeParameters.Any())
            {
                for (int i = 0; i < declaration.TypeParameters.Length; i++)
                {
                    int newIndex = TypeIndex++;
                    result.TypeParameters[declaration.TypeParameters[i]] = new UnknownType(newIndex);
                    lookup.Add(declaration.TypeParameters[i], newIndex);
                }
            }

            int index = TypeIndex++;

            KnownTypes[index] = PartialTypeCreator.Convert(declaration.Type, 
                ident => new UnknownType(lookup[ident]));

            return Tuple.Create<IConstrainedData, int>(result, index);          
        }

        public Tuple<IConstrainedData, int> VisitIdentity(Compiler.Data.Identity d)
        {
            int t = TypeIndex++;
            int arrow = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Identity();
            result.Type = new UnknownType(t);

            Constraints.Add(new ArrowConstraint(t, t, arrow));

            return Tuple.Create<IConstrainedData, int>(result, arrow);
        }

        public Tuple<IConstrainedData, int> VisitInl(Compiler.Data.Inl d)
        {
            int t1 = TypeIndex++;
            int t2 = TypeIndex++;
            int sum = TypeIndex++;
            int arrow = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Inl();
            result.LeftType = new UnknownType(t1);
            result.RightType = new UnknownType(t2);

            Constraints.Add(new SumConstraint(t1, t2, sum));
            Constraints.Add(new ArrowConstraint(t1, sum, arrow));

            return Tuple.Create<IConstrainedData, int>(result, arrow);
        }

        public Tuple<IConstrainedData, int> VisitInr(Compiler.Data.Inr d)
        {
            int t1 = TypeIndex++;
            int t2 = TypeIndex++;
            int sum = TypeIndex++;
            int arrow = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Inr();
            result.LeftType = new UnknownType(t1);
            result.RightType = new UnknownType(t2);

            Constraints.Add(new SumConstraint(t1, t2, sum));
            Constraints.Add(new ArrowConstraint(t2, sum, arrow));

            return Tuple.Create<IConstrainedData, int>(result, arrow);
        }

        public Tuple<IConstrainedData, int> VisitOutl(Compiler.Data.Outl d)
        {
            int t1 = TypeIndex++;
            int t2 = TypeIndex++;
            int product = TypeIndex++;
            int arrow = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Outl();
            result.LeftType = new UnknownType(t1);
            result.RightType = new UnknownType(t2);

            Constraints.Add(new ProductConstraint(t1, t2, product));
            Constraints.Add(new ArrowConstraint(product, t1, arrow));

            return Tuple.Create<IConstrainedData, int>(result, arrow);
        }

        public Tuple<IConstrainedData, int> VisitOutr(Compiler.Data.Outr d)
        {
            int t1 = TypeIndex++;
            int t2 = TypeIndex++;
            int product = TypeIndex++;
            int arrow = TypeIndex++;

            var result = new Purity.Compiler.Typechecker.Data.Outr();
            result.LeftType = new UnknownType(t1);
            result.RightType = new UnknownType(t2);

            Constraints.Add(new ProductConstraint(t1, t2, product));
            Constraints.Add(new ArrowConstraint(product, t2, arrow));

            return Tuple.Create<IConstrainedData, int>(result, arrow);
        }

        public Tuple<IConstrainedData, int> VisitSplit(Compiler.Data.Split d)
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

            Constraints.Add(new ProductConstraint(t1, t2, product));
            Constraints.Add(new ArrowConstraint(input, product, arrow));
            Constraints.Add(new ArrowConstraint(input, t1, left));
            Constraints.Add(new ArrowConstraint(input, t2, right));

            return Tuple.Create<IConstrainedData, int>(result, arrow);
        }

        public Tuple<IConstrainedData, int> VisitCurry(Compiler.Data.Curried curried)
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

            Constraints.Add(new ProductConstraint(first, second, product));
            Constraints.Add(new ArrowConstraint(second, output, innerArrow));
            Constraints.Add(new ArrowConstraint(first, innerArrow, arrow));
            Constraints.Add(new ArrowConstraint(product, output, uncurried));

            return Tuple.Create<IConstrainedData, int>(result, arrow);
        }

        public Tuple<IConstrainedData, int> VisitUncurry(Compiler.Data.Uncurried uncurried)
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

            Constraints.Add(new ProductConstraint(first, second, product));
            Constraints.Add(new ArrowConstraint(second, output, innerArrow));
            Constraints.Add(new ArrowConstraint(first, innerArrow, curried));
            Constraints.Add(new ArrowConstraint(product, output, arrow));

            return Tuple.Create<IConstrainedData, int>(result, arrow);
        }

        public Tuple<IConstrainedData, int> VisitAbstraction(Compiler.Data.Abstraction d)
        {
            int bodyType;
            int variableType = TypeIndex++;
            int arrow = TypeIndex++;

            Variables.Add(d.Variable, variableType);

            var result = new Purity.Compiler.Typechecker.Data.Abstraction(d.Variable, Convert(d.Body, out bodyType));
            result.BodyType = new UnknownType(bodyType);
            result.VariableType = new UnknownType(variableType);

            Constraints.Add(new ArrowConstraint(variableType, bodyType, arrow));

            return Tuple.Create<IConstrainedData, int>(result, arrow);
        }

        public Tuple<IConstrainedData, int> VisitVariable(Compiler.Data.Variable d)
        {
            if (!Variables.ContainsKey(d.Name))
            {
                throw new CompilerException(string.Format(ErrorMessages.UnexpectedVariable, d.Name));
            }

            var result = new Purity.Compiler.Typechecker.Data.Variable(d.Name);
            result.Type = new UnknownType(Variables[d.Name]);

            return Tuple.Create<IConstrainedData, int>(result, Variables[d.Name]);
        }
    }
}
