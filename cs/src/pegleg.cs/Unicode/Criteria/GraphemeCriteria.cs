using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode.Criteria {
	/// <summary>
	/// Class which can be configured to accept or reject unicode graphemes according to
	/// an initial sense and configured exclusions.
	/// </summary>
	public class GraphemeCriteria : IUnicodeCriteria {
		/// <summary>
		/// Create a base <see cref="GraphemeCriteria"/> which accepts all unicode graphemes.
		/// </summary>
		public static GraphemeCriteria Any { get { return new GraphemeCriteria(true); } }

		/// <summary>
		/// Create a base <see cref="GraphemeCriteria"/> which accepts no unicode graphemes.
		/// </summary>
		public static GraphemeCriteria None { get { return new GraphemeCriteria(false); } }

		private readonly bool _sense;
		private readonly IGraphemeCriterion[] _exclusions;

		private GraphemeCriteria(bool sense) {
			_sense = sense;
			_exclusions = new IGraphemeCriterion[0];
		}

		private GraphemeCriteria(GraphemeCriteria prototype, IGraphemeCriterion exclusion) {
			_sense = prototype._sense;

			_exclusions = new IGraphemeCriterion[prototype._exclusions.Length + 1];
			var i = 0;
			while(i < prototype._exclusions.Length) {
				_exclusions[i] = prototype._exclusions[i];
				i++;
			}

			_exclusions[i] = exclusion;
		}

		private GraphemeCriteria CloneExcluding(IGraphemeCriterion exclusion) {
			return new GraphemeCriteria(this, exclusion);
		}

		/// <summary>
		/// Returns true if the criteria are satisfied by the grapheme at <paramref name="index"/> in <paramref name="str"/>.
		/// If satisfied, <paramref name="length"/> is assigned the length of the satisfying grapheme.
		/// </summary>
		public bool AreSatisfiedBy(string str, int index, out int length) {
			var category = UnicodeUtils.GetGraphemeInfo(str, index, out length);

			for(var i = 0; i < _exclusions.Length; i++)
				if(_exclusions[i].IsSatisfiedBy(str, index, length, category))
					return !_sense;

			return _sense;
		}

		/// <summary>
		/// Creates a new <see cref="GraphemeCriteria"/> which also excludes graphemes for which all constituent code points
		/// satisfy the provided <paramref name="codePointCriteria"/>.
		/// </summary>
		public GraphemeCriteria Excluding(CodePointCriteria codePointCriteria) {
			return CloneExcluding(new GraphemeCodePointsCriterion(codePointCriteria));
		}

		/// <summary>
		/// Creates a new <see cref="GraphemeCriteria"/> which also excludes graphemes in the provided <paramref name="categories"/>.
		/// </summary>
		public GraphemeCriteria Excluding(params UnicodeCategory[] categories) {
			return Excluding(categories.AsEnumerable());
		}

		/// <summary>
		/// Creates a new <see cref="GraphemeCriteria"/> which also excludes graphemes in the provided <paramref name="categories"/>.
		/// </summary>
		public GraphemeCriteria Excluding(params IEnumerable<UnicodeCategory>[] categories) {
			return CloneExcluding(new GraphemeInCategoriesCriterion(categories.Flatten()));
		}

		/// <summary>
		/// Creates a new <see cref="GraphemeCriteria"/> which also excludes graphemes present in the provided set of <paramref name="graphemes"/>.
		/// </summary>
		public GraphemeCriteria Excluding(params char[] graphemes) {
			return Excluding(graphemes.Select(c => c.ToString()));
		}

		/// <summary>
		/// Creates a new <see cref="GraphemeCriteria"/> which also excludes graphemes present in the provided set of <paramref name="graphemes"/>.
		/// </summary>
		public GraphemeCriteria Excluding(params IEnumerable<char>[] graphemes) {
			return Excluding(graphemes.Flatten().Select(c => c.ToString()));
		}

		/// <summary>
		/// Creates a new <see cref="GraphemeCriteria"/> which also excludes graphemes present in the provided set of <paramref name="graphemes"/>.
		/// </summary>
		public GraphemeCriteria Excluding(params string[] graphemes) {
			return Excluding(graphemes.AsEnumerable());
		}

		/// <summary>
		/// Creates a new <see cref="GraphemeCriteria"/> which also excludes graphemes present in the provided set of <paramref name="graphemes"/>.
		/// </summary>
		public GraphemeCriteria Excluding(params IEnumerable<string>[] graphemes) {
			return CloneExcluding(new GraphemeInValuesCriterion(graphemes.Flatten()));
		}
	}
}
