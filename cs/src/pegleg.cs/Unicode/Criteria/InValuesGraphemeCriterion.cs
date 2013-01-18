using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode.Criteria {
	/// <summary>
	/// Criterion wihch is satisfied by graphemes present in a provided set of graphemes.
	/// </summary>
	public class InValuesGraphemeCriterion : IGraphemeCriteria {
		private readonly Trie<char, string> _trie;

		/// <summary>
		/// Create a criterion which is satisfied by graphemes present in the provided set of <paramref name="graphemes"/>.
		/// </summary>
		public InValuesGraphemeCriterion(IEnumerable<string> graphemes) {
			if(null == graphemes) throw Xception.Because.ArgumentNull(() => graphemes);
			if(!graphemes.Any()) throw Xception.Because.Argument(() => graphemes, "cannot be empty");

			_trie = new Trie<char, string>();
			foreach(var grapheme in graphemes) {
				// Check that the string is a single grapheme
				int length;
				UnicodeUtils.GetGraphemeInfo(grapheme, 0, out length);
				if(grapheme.Length != length)
					throw Xception.Because.Argument(() => graphemes, "contains invalid grapheme " + StringUtils.LiteralEncode(grapheme));

				_trie.SetValue(grapheme, grapheme);
			}
		}

		public bool SatisfiedBy(string str, int index, int length, UnicodeCategory category) {
			var endIndex = index + length;

			var subTrie = _trie;
			while(index != endIndex)
				if(!subTrie.TryGetSubtrie(str[index++], out subTrie))
					return false;

			return subTrie.HasValue;
		}
	}
}
