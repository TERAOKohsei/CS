﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;

namespace CS.CommonRc {
    using Binary = Func<ParameterExpression, ParameterExpression, BinaryExpression>;

    public static class Operator<T> {
        private static readonly ParameterExpression x = Expression.Parameter(typeof(T), "x");
        private static readonly ParameterExpression y = Expression.Parameter(typeof(T), "y");
        public static readonly Func<T, T, T> Add = Lambda(Expression.Add);
        public static readonly Func<T, T, T> Subtract = Lambda(Expression.Subtract);
        public static readonly Func<T, T, T> Multiply = Lambda(Expression.Multiply);
        public static readonly Func<T, T, T> Divide = Lambda(Expression.Divide);

        public static Func<T, T, T> Lambda(Binary op) {
            return Expression.Lambda<Func<T, T, T>>(
                op(x, y),
                x, y).Compile();
        }
    }

}
