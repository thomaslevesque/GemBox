using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GemBox.Collections;

namespace GemBox.UnitTests
{
    public class TrieTests
    {
        public class ArgumentValidationTests
        {
            public void Add_Should_Throw_If_Key_Is_Null()
            {
                // ReSharper disable once CollectionNeverQueried.Local
                var trie = new Trie<int>();
                Assert.ThrowsWhenArgumentNull(() => trie.Add("foo", 42), "key");
            }

            public void Remove_Should_Throw_If_Key_Is_Null()
            {
                var trie = new Trie<int>();
                Assert.ThrowsWhenArgumentNull(() => trie.Remove("foo"), "key");
            }

            public void ContainsKey_Should_Throw_If_Key_Is_Null()
            {
                // ReSharper disable once CollectionNeverUpdated.Local
                var trie = new Trie<int>();
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Assert.ThrowsWhenArgumentNull(() => trie.ContainsKey("foo"), "key");
            }

            public void Indexer_Get_Should_Throw_If_Key_Is_Null()
            {
                // ReSharper disable once CollectionNeverUpdated.Local
                var trie = new Trie<int>();
                string key = null;
                // ReSharper disable once ExpressionIsAlwaysNull
                Action action = () => trie[key].Ignore();
                action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("key");
            }

            public void Indexer_Set_Should_Throw_If_Key_Is_Null()
            {
                // ReSharper disable once CollectionNeverQueried.Local
                var trie = new Trie<int>();
                string key = null;
                // ReSharper disable once ExpressionIsAlwaysNull
                Action action = () => trie[key] = 42;
                action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("key");
            }

            public void AddOrUpdate_Should_Throw_If_Args_Are_Null()
            {
                var trie = new Trie<int>();
                Assert.ThrowsWhenArgumentNull(() => trie.AddOrUpdate("foo", 0, (key, value) => 0), "key", "updateValueFactory");
            }

            public void AddOrUpdate_With_Add_Value_Func_Should_Throw_If_Args_Are_Null()
            {
                var trie = new Trie<int>();
                Assert.ThrowsWhenArgumentNull(() => trie.AddOrUpdate("foo", key => 0, (key, value) => 0), "key", "updateValueFactory");
            }

            public void ContainsPrefix_Should_Throw_If_Prefix_Is_Null()
            {
                var trie = new Trie<int>();
                Assert.ThrowsWhenArgumentNull(() => trie.ContainsPrefix("foo"), "prefix");
            }

            public void FindPrefix_Should_Throw_If_Prefix_Is_Null()
            {
                var trie = new Trie<int>();
                Assert.ThrowsWhenArgumentNull(() => trie.FindPrefix("foo"), "prefix");
            }

            public void RemovePrefix_Should_Throw_If_Prefix_Is_Null()
            {
                var trie = new Trie<int>();
                Assert.ThrowsWhenArgumentNull(() => trie.RemovePrefix("foo"), "prefix");
            }
        }

        public class DictionaryImplTests
        {
            public void Enumerating_Trie_Should_Return_All_Items()
            {
                var trie = new Trie<int> {{"a", 42}, {"ab", 123}, {"abc", 999}, {"foo", 0}};
                var expected = new Dictionary<string, int> { { "a", 42 }, { "ab", 123 }, { "abc", 999 }, { "foo", 0 } };
                var actual = trie.ToList();
                actual.Should().BeEquivalentTo(expected);
            }

            public void Add_Should_Add_Value()
            {
                var trie = new Trie<int>();
                trie.Add("foo", 42);
                trie.Count.Should().Be(1);
            }

            public void Add_Should_Throw_If_Key_Already_Present()
            {
                // ReSharper disable once CollectionNeverQueried.Local
                var trie = new Trie<int>();
                trie.Add("foo", 42);
                Action action = () => trie.Add("foo", 123);
                action.ShouldThrow<ArgumentException>();
            }

            public void Remove_Should_Return_True_And_Remove_Key_If_Key_Is_Found()
            {
                var trie = new Trie<int> {{"foo", 42}, {"bar", 123}};
                trie.Remove("bar").Should().BeTrue();
                trie.ContainsKey("foo").Should().BeTrue();
                trie.ContainsKey("bar").Should().BeFalse();
                trie.Count.Should().Be(1);
            }

            public void Remove_Should_Return_False_And_Remove_Nothing_If_Key_Is_Not_Found()
            {
                var trie = new Trie<int> { { "foo", 42 }, { "bar", 123 } };
                trie.Remove("baz").Should().BeFalse();
                trie.ContainsKey("foo").Should().BeTrue();
                trie.ContainsKey("bar").Should().BeTrue();
                trie.ContainsKey("baz").Should().BeFalse();
                trie.Count.Should().Be(2);
            }

            public void Indexer_Should_Return_Corresponding_Key()
            {
                var trie = new Trie<int> {{"a", 42}, {"ab", 123}, {"abc", 999}, {"foo", 0}};
                trie["a"].Should().Be(42);
                trie["ab"].Should().Be(123);
                trie["abc"].Should().Be(999);
                trie["foo"].Should().Be(0);
            }

            public void Indexer_Should_Throw_If_Key_Is_Not_Found()
            {
                var trie = new Trie<int> {{"a", 42}, {"ab", 123}, {"abc", 999}, {"foo", 0}};
                Action action = () => trie["bar"].Ignore();
                action.ShouldThrow<KeyNotFoundException>();
            }

            public void Indexer_Should_Add_Value_If_Key_Is_Not_Found()
            {
                var trie = new Trie<int>();
                trie["foo"] = 15;
                trie.Count.Should().Be(1);
                trie["foo"].Should().Be(15);
            }

            public void Indexer_Should_Replace_Value_If_Key_Is_Found()
            {
                var trie = new Trie<int> {{"foo", 15}};
                trie["foo"] = 42;
                trie.Count.Should().Be(1);
                trie["foo"].Should().Be(42);
            }

            public void ContainsKey_Should_Return_True_If_Key_Is_Found()
            {
                var trie = new Trie<int> {{"a", 42}, {"ab", 123}, {"abc", 999}, {"foo", 0}};
                trie.ContainsKey("abc").Should().BeTrue();
            }

            public void ContainsKey_Should_Return_False_If_Key_Is_Not_Found()
            {
                var trie = new Trie<int> { { "a", 42 }, { "ab", 123 }, { "abc", 999 }, { "foo", 0 } };
                trie.ContainsKey("bar").Should().BeFalse();
            }

            public void TryGetValue_Should_Return_True_And_Get_Value_If_Key_Is_Found()
            {
                var trie = new Trie<int> { { "a", 42 }, { "ab", 123 }, { "abc", 999 }, { "foo", 0 } };
                int x;
                trie.TryGetValue("a", out x).Should().BeTrue();
                x.Should().Be(42);
            }

            public void TryGetValue_Should_Return_False_And_Get_Default_Value_If_Key_Is_Not_Found()
            {
                var trie = new Trie<int> { { "a", 42 }, { "ab", 123 }, { "abc", 999 }, { "foo", 0 } };
                int x;
                trie.TryGetValue("bar", out x).Should().BeFalse();
                x.Should().Be(0);
            }

            public void Clear_Should_Remove_All_Keys()
            {
                var trie = new Trie<int> { { "a", 42 }, { "ab", 123 }, { "abc", 999 }, { "foo", 0 } };
                trie.Clear();
                trie.Count.Should().Be(0);
            }

            public void Keys_Should_Return_All_Keys()
            {
                var trie = new Trie<int> { { "a", 42 }, { "ab", 123 }, { "abc", 999 }, { "foo", 0 } };
                var expected = new[] {"a", "ab", "abc", "foo"};
                trie.Keys.Should().BeEquivalentTo(expected);
            }

            public void Values_Should_Return_All_Values()
            {
                var trie = new Trie<int> { { "a", 42 }, { "ab", 123 }, { "abc", 999 }, { "foo", 0 } };
                var expected = new[] { 42, 123, 999, 0 };
                trie.Values.Should().BeEquivalentTo(expected);
            }
        }

        public class TrieImplTests
        {
            public void ContainsPrefix_Should_Return_True_If_Trie_Contains_Prefix()
            {
                var trie = new Trie<int> {{"abc", 42}, {"abd", 123}};
                trie.ContainsPrefix("a").Should().BeTrue();
                trie.ContainsPrefix("ab").Should().BeTrue();
            }

            public void ContainsPrefix_Should_Return_False_If_Trie_Doesnt_Contain_Prefix()
            {
                var trie = new Trie<int> {{"abc", 42}, {"abd", 123}};
                trie.ContainsPrefix("b").Should().BeFalse();
                trie.ContainsPrefix("abx").Should().BeFalse();
            }

            public void FindPrefix_Should_Return_All_Items_With_Prefix()
            {
                var trie = new Trie<int>
                {
                    {"abc", 42},
                    {"foo", 123},
                    {"abd", 1},
                    {"bar", 2},
                    {"abcd", 99},
                    {"azerty", 3},
                };

                var expected = new Dictionary<string, int>
                {
                    {"abc", 42},
                    {"abd", 1},
                    {"abcd", 99},
                    {"azerty", 3},
                };
                var actual = trie.FindPrefix("a");
                actual.Should().BeEquivalentTo(expected);

                expected = new Dictionary<string, int>
                {
                    {"abc", 42},
                    {"abd", 1},
                    {"abcd", 99},
                };
                actual = trie.FindPrefix("ab");
                actual.Should().BeEquivalentTo(expected);

                expected = new Dictionary<string, int>
                {
                    {"abc", 42},
                    {"abcd", 99},
                };
                actual = trie.FindPrefix("abc");
                actual.Should().BeEquivalentTo(expected);

                expected = new Dictionary<string, int>
                {
                    {"foo", 123},
                };
                actual = trie.FindPrefix("foo");
                actual.Should().BeEquivalentTo(expected);
            }

            public void FindPrefix_Should_Return_Empty_Sequence_If_No_Items_Match()
            {
                var trie = new Trie<int>
                {
                    {"abc", 42},
                    {"foo", 123},
                    {"abd", 1},
                    {"bar", 2},
                    {"abcd", 99},
                    {"azerty", 3},
                };

                trie.FindPrefix("baz").Should().BeEmpty();
            }

            public void FindPrefix_With_Empty_Prefix_Should_Return_Everything()
            {
                var trie = new Trie<int>
                {
                    {"abc", 42},
                    {"foo", 123},
                    {"abd", 1},
                    {"bar", 2},
                    {"abcd", 99},
                    {"azerty", 3},
                };

                trie.FindPrefix("").Should().BeEquivalentTo(trie);
            }

            public void RemovePrefix_Should_Remove_All_Matching_Keys()
            {
                var trie = new Trie<int>
                {
                    {"abc", 42},
                    {"foo", 123},
                    {"abd", 1},
                    {"bar", 2},
                    {"abcd", 99},
                    {"azerty", 3},
                };

                trie.RemovePrefix("a").Should().Be(4);
                trie.ContainsKey("abc").Should().BeFalse();
                trie.ContainsKey("abd").Should().BeFalse();
                trie.ContainsKey("abcd").Should().BeFalse();
                trie.ContainsKey("azerty").Should().BeFalse();
                trie.ContainsPrefix("a").Should().BeFalse();
                trie.Count.Should().Be(2);
            }

            public void RemovePrefix_Should_Remove_Nothing_If_Key_Is_Not_Found()
            {
                var trie = new Trie<int>
                {
                    {"abc", 42},
                    {"foo", 123},
                    {"abd", 1},
                    {"bar", 2},
                    {"abcd", 99},
                    {"azerty", 3},
                };

                trie.RemovePrefix("baz").Should().Be(0);
                trie.ContainsKey("abc").Should().BeTrue();
                trie.ContainsKey("foo").Should().BeTrue();
                trie.ContainsKey("abd").Should().BeTrue();
                trie.ContainsKey("bar").Should().BeTrue();
                trie.ContainsKey("abcd").Should().BeTrue();
                trie.ContainsKey("azerty").Should().BeTrue();
                trie.Count.Should().Be(6);
            }

            public void AddOrUpdate_Should_Add_Value_And_Not_Call_Update_Func_If_Key_Not_Found()
            {
                var trie = new Trie<int>();
                bool updated = false;
                int result = trie.AddOrUpdate("foo", 42, (key, value) =>
                {
                    updated = true;
                    return 0;
                });
                result.Should().Be(42);
                trie["foo"].Should().Be(42);
                trie.Count.Should().Be(1);
                updated.Should().BeFalse();
            }

            public void AddOrUpdate_Should_Update_Value_If_Key_Is_Found()
            {
                var trie = new Trie<int> {{"foo", 41}};
                bool updated = false;
                int result = trie.AddOrUpdate("foo", 42, (key, value) =>
                {
                    updated = true;
                    return value + 1;
                });
                result.Should().Be(42);
                trie["foo"].Should().Be(42);
                trie.Count.Should().Be(1);
                updated.Should().BeTrue();
            }

            public void AddOrUpdate_Should_Generate_Value_And_Not_Call_Update_Func_If_Key_Not_Found()
            {
                var trie = new Trie<int>();
                bool generated = false;
                bool updated = false;
                int result = trie.AddOrUpdate(
                    "foo",
                    key =>
                    {
                        generated = true;
                        return 42;
                    },
                    (key, value) =>
                    {
                        updated = true;
                        return 0;
                    });
                result.Should().Be(42);
                trie["foo"].Should().Be(42);
                trie.Count.Should().Be(1);
                generated.Should().BeTrue();
                updated.Should().BeFalse();
            }

            public void AddOrUpdate_Should_Update_Value_And_Not_Call_Generator_Func_If_Key_Is_Found()
            {
                var trie = new Trie<int> { {"foo",  41} };
                bool generated = false;
                bool updated = false;
                int result = trie.AddOrUpdate(
                    "foo",
                    key =>
                    {
                        generated = true;
                        return 99;
                    },
                    (key, value) =>
                    {
                        updated = true;
                        return value + 1;
                    });
                result.Should().Be(42);
                trie["foo"].Should().Be(42);
                trie.Count.Should().Be(1);
                generated.Should().BeFalse();
                updated.Should().BeTrue();
            }
        }
    }
}
