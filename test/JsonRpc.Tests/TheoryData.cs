using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JsonRpc.Tests
{
    public abstract class TheoryData : IEnumerable<object[]>
    {
        public abstract IEnumerator<object[]> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public abstract class TheoryData<T> : TheoryData
    {
        public override IEnumerator<object[]> GetEnumerator()
        {
            return GetValues().Select(x => new object[] { x.Item1 }).GetEnumerator();
        }

        public abstract IEnumerable<ValueTuple<T>> GetValues();
    }

    public abstract class TheoryData<T1, T2> : TheoryData
    {
        public override IEnumerator<object[]> GetEnumerator()
        {
            return GetValues().Select(x => new object[] { x.Item1, x.Item2 }).GetEnumerator();
        }

        public abstract IEnumerable<ValueTuple<T1, T2>> GetValues();
    }

    public abstract class TheoryData<T1, T2, T3> : TheoryData
    {
        public override IEnumerator<object[]> GetEnumerator()
        {
            return GetValues().Select(x => new object[] { x.Item1, x.Item2, x.Item3 }).GetEnumerator();
        }

        public abstract IEnumerable<ValueTuple<T1, T2, T3>> GetValues();
    }

    public abstract class TheoryData<T1, T2, T3, T4> : TheoryData
    {
        public override IEnumerator<object[]> GetEnumerator()
        {
            return GetValues().Select(x => new object[] { x.Item1, x.Item2, x.Item3, x.Item4 }).GetEnumerator();
        }

        public abstract IEnumerable<ValueTuple<T1, T2, T3, T4>> GetValues();
    }
}