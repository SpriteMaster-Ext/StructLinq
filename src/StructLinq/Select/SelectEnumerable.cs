﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StructLinq.IEnumerable;

namespace StructLinq.Select
{
    public struct SelectEnumerable<TIn, TOut, TEnumerable, TEnumerator, TFunction> : IStructEnumerable<TOut, SelectEnumerator<TIn, TOut, TEnumerator, TFunction>>
        where TFunction : struct, IFunction<TIn, TOut>
        where TEnumerator : struct, IStructEnumerator<TIn>
        where TEnumerable : struct, IStructEnumerable<TIn, TEnumerator>
    {
        #region private fields
        private TFunction function;
        private TEnumerable inner;
        #endregion

        public SelectEnumerable(ref TFunction function, ref TEnumerable inner)
        {
            this.function = function;
            this.inner = inner;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SelectEnumerator<TIn, TOut, TEnumerator, TFunction> GetStructEnumerator()
        {
            var typedEnumerator = inner.GetStructEnumerator();
            return new SelectEnumerator<TIn, TOut, TEnumerator, TFunction>(ref function, ref typedEnumerator);
        }

        /// <summary>
        ///An enumerator, duck-typing-compatible with foreach.
        /// </summary>
        public SelectEnumerator<TIn, TOut, TEnumerator, TFunction> GetEnumerator()
        {
            return GetStructEnumerator();
        }


        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new StructEnumerator<TOut, SelectEnumerator<TIn, TOut, TEnumerator, TFunction>>(GetStructEnumerator());
        }

        IEnumerator<TOut> IEnumerable<TOut>.GetEnumerator()
        {
            return new StructEnumerator<TOut, SelectEnumerator<TIn, TOut, TEnumerator, TFunction>>(GetStructEnumerator());
        }
    }
}

    
