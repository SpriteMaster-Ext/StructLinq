﻿using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StructLinq.Utils.Collections;

namespace StructLinq.Distinct
{
    public struct DistinctEnumerable<T, TEnumerable, TEnumerator, TComparer> : IStructEnumerable<T,
            DistinctEnumerator<T, TEnumerator, TComparer>>
        where TComparer : IEqualityComparer<T>
        where TEnumerator : struct, IStructEnumerator<T>
        where TEnumerable : IStructEnumerable<T, TEnumerator>

    {
        private TEnumerable enumerable;
        private readonly TComparer comparer;
        private readonly int capacity;
        private readonly ArrayPool<int> bucketPool;
        private readonly ArrayPool<Slot<T>> slotPool;
        public DistinctEnumerable(ref TEnumerable enumerable, TComparer comparer, int capacity,
            ArrayPool<int> bucketPool, ArrayPool<Slot<T>> slotPool)
        {
            this.enumerable = enumerable;
            this.comparer = comparer;
            this.capacity = capacity;
            this.bucketPool = bucketPool;
            this.slotPool = slotPool;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DistinctEnumerator<T, TEnumerator, TComparer> GetEnumerator()
        {
            var enumerator = enumerable.GetEnumerator();
            return new DistinctEnumerator<T, TEnumerator, TComparer>(ref enumerator, capacity, bucketPool, slotPool, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VisitStatus Visit<TVisitor>(ref TVisitor visitor)
            where TVisitor : IVisitor<T>
        {
            var distinctVisitor = new DistinctVisitor<TVisitor>(capacity, bucketPool, slotPool, comparer, ref visitor);
            var visitStatus = enumerable.Visit(ref distinctVisitor);
            visitor = distinctVisitor.visitor;
            distinctVisitor.Dispose();
            return visitStatus;
        }

        private struct DistinctVisitor<TVisitor> : IVisitor<T>
            where TVisitor : IVisitor<T>
        {
            public TVisitor visitor;
            private PooledSet<T, TComparer> set;

            public DistinctVisitor(int capacity, ArrayPool<int> bucketPool, ArrayPool<Slot<T>> slotPool, TComparer comparer, ref TVisitor visitor)
            {
                this.visitor = visitor;
                set = new PooledSet<T, TComparer>(capacity, bucketPool, slotPool, comparer);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Visit(T input)
            {
                return !set.AddIfNotPresent(input) || visitor.Visit(input);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose()
            {
                set.Dispose();
            }
        }
    }
}