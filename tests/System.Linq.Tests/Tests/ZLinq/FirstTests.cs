// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using Xunit;

namespace ZLinq.Tests
{
    public class FirstTests : EnumerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            var q = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                    where x > int.MinValue
                    select x;

            Assert.Equal(q.First(), q.First());
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            var q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty }
                    where !string.IsNullOrEmpty(x)
                    select x;

            Assert.Equal(q.First(), q.First());
        }

        private static void TestEmptyIList<T>()
        {
            T[] source = [];

            Assert.NotNull(source as IList<T>);

            Assert.Throws<InvalidOperationException>(() => source.RunOnce().First());
        }

        [Fact]
        public void EmptyIListT()
        {
            TestEmptyIList<int>();
            TestEmptyIList<string>();
            TestEmptyIList<DateTime>();
            TestEmptyIList<FirstTests>();
        }

        [Fact]
        public void IListTOneElement()
        {
            int[] source = [5];
            int expected = 5;

            Assert.NotNull(source as IList<int>);

            Assert.Equal(expected, source.First());
        }

        [Fact]
        public void IListTManyElementsFirstIsDefault()
        {
            int?[] source = [null, -10, 2, 4, 3, 0, 2];
            int? expected = null;

            Assert.IsAssignableFrom<IList<int?>>(source);

            Assert.Equal(expected, source.First());
        }

        [Fact]
        public void IListTManyElementsFirstIsNotDefault()
        {
            int?[] source = [19, null, -10, 2, 4, 3, 0, 2];
            int? expected = 19;

            Assert.IsAssignableFrom<IList<int?>>(source);

            Assert.Equal(expected, source.First());
        }

        private static void TestEmptyNotIList<T>()
        {
            static IEnumerable<T1> EmptySource<T1>()
            {
                yield break;
            }

            var source = EmptySource<T>();

            Assert.Null(source as IList<T>);

            Assert.Throws<InvalidOperationException>(() => source.RunOnce().First());
        }

        [Fact]
        public void EmptyNotIListT()
        {
            TestEmptyNotIList<int>();
            TestEmptyNotIList<string>();
            TestEmptyNotIList<DateTime>();
            TestEmptyNotIList<FirstTests>();
        }

        [Fact]
        public void OneElementNotIListT()
        {
            IEnumerable<int> source = NumberRangeGuaranteedNotCollectionType(-5, 1);
            int expected = -5;

            Assert.Null(source as IList<int>);

            Assert.Equal(expected, source.First());
        }

        [Fact]
        public void ManyElementsNotIListT()
        {
            IEnumerable<int> source = NumberRangeGuaranteedNotCollectionType(3, 10);
            int expected = 3;

            Assert.Null(source as IList<int>);

            Assert.Equal(expected, source.First());
        }

        [Fact]
        public void EmptySource()
        {
            int[] source = [];
            Assert.All(CreateSources(source), source =>
            {
                Assert.Throws<InvalidOperationException>(() => source.First(x => true));
                Assert.Throws<InvalidOperationException>(() => source.First(x => false));
            });
        }

        [Fact]
        public void OneElementTruePredicate()
        {
            int[] source = [4];
            Func<int, bool> predicate = IsEven;
            int expected = 4;

            Assert.All(CreateSources(source), source =>
            {
                Assert.Equal(expected, source.First(predicate));
            });
        }

        [Fact]
        public void ManyElementsPredicateFalseForAll()
        {
            int[] source = [9, 5, 1, 3, 17, 21];
            Func<int, bool> predicate = IsEven;

            Assert.All(CreateSources(source), source =>
            {
                Assert.Throws<InvalidOperationException>(() => source.First(predicate));
            });
        }

        [Fact]
        public void PredicateTrueOnlyForLast()
        {
            int[] source = [9, 5, 1, 3, 17, 21, 50];
            Func<int, bool> predicate = IsEven;
            int expected = 50;

            Assert.All(CreateSources(source), source =>
            {
                Assert.Equal(expected, source.First(predicate));
            });
        }

        [Fact]
        public void PredicateTrueForSome()
        {
            int[] source = [3, 7, 10, 7, 9, 2, 11, 17, 13, 8];
            Func<int, bool> predicate = IsEven;
            int expected = 10;

            Assert.Equal(expected, source.First(predicate));
        }

        [Fact]
        public void PredicateTrueForSomeRunOnce()
        {
            int[] source = [3, 7, 10, 7, 9, 2, 11, 17, 13, 8];
            Func<int, bool> predicate = IsEven;
            int expected = 10;

            Assert.All(CreateSources(source), source =>
            {
                Assert.Equal(expected, source.RunOnce().First(predicate));
            });
        }

        [Fact]
        public void NullSource()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).First());
        }

        [Fact]
        public void NullSourcePredicateUsed()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).First(i => i != 2));
        }

        [Fact]
        public void NullPredicate()
        {
            Func<int, bool> predicate = null;
            AssertExtensions.Throws<ArgumentNullException>("predicate", () => Enumerable.Range(0, 3).First(predicate));
        }
    }
}
