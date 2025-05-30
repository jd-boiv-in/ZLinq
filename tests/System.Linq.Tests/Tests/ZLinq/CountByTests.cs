// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Xunit;

namespace ZLinq.Tests
{
    public class CountByTests : EnumerableTests
    {
        [Fact]
        public void CountBy_SourceNull_ThrowsArgumentNullException()
        {
            string[] first = null;

            AssertExtensions.Throws<ArgumentNullException>("source", () => first.CountBy(x => x));
            AssertExtensions.Throws<ArgumentNullException>("source", () => first.CountBy(x => x, new AnagramEqualityComparer()));
        }

        [Fact]
        public void CountBy_KeySelectorNull_ThrowsArgumentNullException()
        {
            string[] source = ["Bob", "Tim", "Robert", "Chris"];
            Func<string, string> keySelector = null;

            AssertExtensions.Throws<ArgumentNullException>("keySelector", () => source.CountBy(keySelector));
            AssertExtensions.Throws<ArgumentNullException>("keySelector", () => source.CountBy(keySelector, new AnagramEqualityComparer()));
        }

        [Fact]
        public void CountBy_SourceThrowsOnGetEnumerator()
        {
            IEnumerable<int> source = new ThrowsOnGetEnumerator();

            Assert.Throws<InvalidOperationException>(() =>
            {
                var enumerator = source.CountBy(x => x).GetEnumerator();
                enumerator.MoveNext();
            });
        }

        [Fact]
        public void CountBy_SourceThrowsOnMoveNext()
        {
            IEnumerable<int> source = new ThrowsOnMoveNext();

            Assert.Throws<InvalidOperationException>(() =>
            {
                var enumerator = source.CountBy(x => x).GetEnumerator();
                enumerator.MoveNext();
            });
        }

        [Fact]
        public void CountBy_SourceThrowsOnCurrent()
        {
            IEnumerable<int> source = new ThrowsOnCurrentEnumerator();

            Assert.Throws<InvalidOperationException>(() =>
            {
                var enumerator = source.CountBy(x => x).GetEnumerator();
                enumerator.MoveNext();
            });
        }

        [Fact]
        public void CountBy_HasExpectedOutput()
        {
            Validate(
                source: Enumerable.Empty<int>(),
                keySelector: x => x,
                comparer: null,
                expected: []);

            Validate(
                source: Enumerable.Range(0, 10),
                keySelector: x => x,
                comparer: null,
                expected: Enumerable.Range(0, 10).Select(x => new KeyValuePair<int, int>(x, 1)).ToArray());

            Validate(
                source: Enumerable.Range(5, 10),
                keySelector: x => true,
                comparer: null,
                expected: Enumerable.Repeat(true, 1).Select(x => new KeyValuePair<bool, int>(x, 10)).ToArray());

            Validate(
                source: Enumerable.Range(0, 20),
                keySelector: x => x % 5,
                comparer: null,
                expected: Enumerable.Range(0, 5).Select(x => new KeyValuePair<int, int>(x, 4)).ToArray());

            Validate(
                source: Enumerable.Repeat(5, 20),
                keySelector: x => x,
                comparer: null,
                expected: Enumerable.Repeat(5, 1).Select(x => new KeyValuePair<int, int>(x, 20)).ToArray());

            Validate(
                source: ["Bob", "bob", "tim", "Bob", "Tim"],
                keySelector: x => x,
                null,
                expected:
                [
                    new("Bob", 2),
                    new("bob", 1),
                    new("tim", 1),
                    new("Tim", 1)
                ]);

            Validate(
                source: ["Bob", "bob", "tim", "Bob", "Tim"],
                keySelector: x => x,
                StringComparer.OrdinalIgnoreCase,
                expected:
                [
                    new("Bob", 3),
                    new("tim", 2)
                ]);

            Validate(
                source: new (string Name, int Age)[] { ("Tom", 20), ("Dick", 30), ("Harry", 40) },
                keySelector: x => x.Age,
                comparer: null,
                expected: new int[] { 20, 30, 40 }.Select(x => new KeyValuePair<int, int>(x, 1)).ToArray());

            Validate(
                source: new (string Name, int Age)[] { ("Tom", 20), ("Dick", 20), ("Harry", 40) },
                keySelector: x => x.Age,
                comparer: null,
                expected:
                [
                    new(20, 2),
                    new(40, 1)
                ]);

            Validate(
                source: new (string Name, int Age)[] { ("Bob", 20), ("bob", 30), ("Harry", 40) },
                keySelector: x => x.Name,
                comparer: null,
                expected: new string[] { "Bob", "bob", "Harry" }.Select(x => new KeyValuePair<string, int>(x, 1)).ToArray());

            Validate(
                source: new (string Name, int Age)[] { ("Bob", 20), ("bob", 30), ("Harry", 40) },
                keySelector: x => x.Name,
                comparer: StringComparer.OrdinalIgnoreCase,
                expected:
                [
                    new("Bob", 2),
                    new("Harry", 1)
                ]);

            static void Validate<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer, IEnumerable<KeyValuePair<TKey, int>> expected)
            {
                Assert.Equal(expected, source.CountBy(keySelector, comparer));
                Assert.Equal(expected, source.RunOnce().CountBy(keySelector, comparer));
            }
        }
    }
}
