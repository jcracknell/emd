using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;

namespace pegleg.cs.Utils {
	public class TrieTests {
		[Fact] public void Trie_TryGetValue_should_work() {
			var trie = new Trie<char, int>();
			trie.SetValue("foo", 42);
		
			int value;
			trie.TryGetValue("foo", out value).Should().BeTrue();
			value.Should().Be(42);
		}

		[Fact] public void Trie_TryGetValue_should_work_for_paths_with_shared_elements() {
			var trie = new Trie<char, int>();
			trie.SetValue("foo", 42);
			trie.SetValue("foobar", 13);
			trie.SetValue("fizban", 53);
			trie.SetValue("fizbang", 12);

			int value;
			trie.TryGetValue("foo", out value).Should().BeTrue();
			value.Should().Be(42);
			trie.TryGetValue("foobar", out value).Should().BeTrue();
			value.Should().Be(13);
			trie.TryGetValue("fizban", out value).Should().BeTrue();
			value.Should().Be(53);
			trie.TryGetValue("fizbang", out value).Should().BeTrue();
			value.Should().Be(12);
		}

		[Fact] public void Trie_TryGetValue_should_work_for_empty_path_with_nonempty_trie() {
			var trie = new Trie<char, int>();
			trie.SetValue("foo", 42);
			trie.SetValue("bar", 13);

			int value;
			trie.TryGetValue("", out value).Should().BeFalse();
		}

		[Fact] public void Trie_TryGetValue_should_work_for_empty_path() {
			var trie = new Trie<char, int>();
			trie.SetValue("", 42);

			trie.HasValue.Should().BeTrue();
			trie.Value.Should().Be(42);

			int value;
			trie.TryGetValue("", out value).Should().BeTrue();
			value.Should().Be(42);
		}

		[Fact] public void Trie_TryGetValue_should_work_for_empty_path_with_empty_trie() {
			var trie = new Trie<char, int>();

			int value;
			trie.TryGetValue("", out value).Should().BeFalse();
		}

		[Fact] public void Trie_TryGetSubtrie_should_work() {
			var trie = new Trie<char, int>();
			trie.SetValue("foo", 42);

			Trie<char, int> subtrie = trie;
			subtrie.TryGetSubtrie('f', out subtrie).Should().BeTrue();
			subtrie.TryGetSubtrie('o', out subtrie).Should().BeTrue();
			subtrie.TryGetSubtrie('o', out subtrie).Should().BeTrue();

			subtrie.Should().NotBeNull();
			subtrie.HasValue.Should().BeTrue();
			subtrie.Value.Should().Be(42);
		}
	}
}
