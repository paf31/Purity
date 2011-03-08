using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core.Types
{
    public class Either<T1, T2>
    {
        private int flag;

        public T1 Left { get; set; }

        public T2 Right { get; set; }

        private Either() { }

        public static Either<T1, T2> Inl(T1 v1)
        {
            return new Either<T1, T2> { flag = 0, Left = v1 };
        }

        public static Either<T1, T2> Inr(T2 v2)
        {
            return new Either<T1, T2> { flag = 1, Right = v2 };
        }

        public R Case<R>(Func<T1, R> f1, Func<T2, R> f2)
        {
            switch (flag)
            {
                case 0: return f1(Left);
                case 1: return f2(Right);
            }

            throw new InvalidOperationException();
        }
    }
}
