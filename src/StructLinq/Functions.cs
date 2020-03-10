﻿using System;

namespace StructLinq
{
    public static class Functions
    {
        public static StructFunction<TIn, TOut> ToStruct<TIn, TOut>(this Func<TIn, TOut> function)
        {
            return new StructFunction<TIn, TOut>(function);
        }
    }

    public struct StructFunction<TIn, TOut> : IFunction<TIn, TOut>
    {
        #region private fields
        private readonly Func<TIn, TOut> inner;
        #endregion
        public StructFunction(Func<TIn, TOut> inner)
        {
            this.inner = inner;
        }
        public readonly TOut Eval(TIn element)
        {
            return inner(element);
        }
    }
}