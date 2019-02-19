﻿using System;
using System.Collections.Generic;
using StructLinq.Where;

// ReSharper disable once CheckNamespace
namespace StructLinq
{
    public static partial class StructEnumerable
    {
        public static ITypedEnumerable<T, WhereEnumerator<T, TEnumerator, TFunction>> 
            Where<T, TEnumerator, TFunction>(this ITypedEnumerable<T, TEnumerator> enumerable, ref TFunction predicate)
            where TEnumerator : IEnumerator<T>
            where TFunction : struct, IFunction<T, bool>
        {
            return new WhereEnumerable<T, TEnumerator, TFunction>(ref predicate, enumerable);
        }
        public static ITypedEnumerable<T, WhereEnumerator<T, TEnumerator, TFunction>>
            Where<T, TEnumerator, TFunction>(this ITypedEnumerable<T, TEnumerator> enumerable, IFunctionFactory<T, bool, TFunction> factory)
            where TEnumerator : IEnumerator<T>
            where TFunction : struct, IFunction<T, bool>
        {
            var predicate = factory.Build();
            return new WhereEnumerable<T, TEnumerator, TFunction>(ref predicate, enumerable);
        }
        public static ITypedEnumerable<T, WhereEnumerator<T, TEnumerator, StructFunction<T,bool>>>
            Where<T, TEnumerator>(this ITypedEnumerable<T, TEnumerator> enumerable, Func<T, bool> predicate)
            where TEnumerator : IEnumerator<T>
        {
            var structPredicate = predicate.ToStruct();
            return enumerable.Where(ref structPredicate);
        }
    }
}
