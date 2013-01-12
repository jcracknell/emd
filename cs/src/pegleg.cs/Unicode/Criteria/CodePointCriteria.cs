using pegleg.cs.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode.Criteria {
	/// <summary>
	/// Class which can be configured to accept or reject unicode code points according to
	/// an initial sense and configured exclusions.
	/// </summary>
	public class CodePointCriteria : IUnicodeCriteria {
		/// <summary>
		/// Create a base <see cref="CodePointCriteria"/> which accepts all unicode code points.
		/// </summary>
		public static CodePointCriteria Any { get { return new CodePointCriteria(true); } }

		/// <summary>
		/// Create a base <see cref="CodePointCriteria"/> which accepts no unicode code points.
		/// </summary>
		public static CodePointCriteria None { get { return new CodePointCriteria(false); } }

		private readonly bool _sense;
		private readonly ICodePointCriterion[] _exclusions;

		private CodePointCriteria(bool sense) {
			_sense = sense;
			_exclusions = new ICodePointCriterion[0];
		}

		private CodePointCriteria(CodePointCriteria prototype, ICodePointCriterion addedExclusion) {
			_sense = prototype._sense;

			_exclusions = new ICodePointCriterion[prototype._exclusions.Length + 1];
			var i = 0;
			while(i < prototype._exclusions.Length) {
				_exclusions[i] = prototype._exclusions[i];
				i++;
			}

			_exclusions[i] = addedExclusion;
		}

		private CodePointCriteria CloneExcluding(ICodePointCriterion exclusion) {
			return new CodePointCriteria(this, exclusion);
		}

		public bool AreSatisfiedBy(string str, int index, out int length) {
			var codePoint = UnicodeUtils.GetCodePoint(str, index, out length);

			for(var i = 0; i < _exclusions.Length; i++)
				if(_exclusions[i].IsSatisfiedBy(codePoint))
					return !_sense;

			return _sense;
		}

		/// <summary>
		/// Creates a new <see cref="CodePointCriteria"/> which also excludes code points in the provided set of <paramref name="categories"/>.
		/// </summary>
		public CodePointCriteria Excluding(params UnicodeCategory[] categories) {
			return Excluding(categories.AsEnumerable());
		}

		/// <summary>
		/// Creates a new <see cref="CodePointCriteria"/> which also excludes code points in the provided set of <paramref name="categories"/>.
		/// </summary>
		public CodePointCriteria Excluding(params IEnumerable<UnicodeCategory>[] categories) {
			return CloneExcluding(new CodePointInCategoriesCriterion(categories.Flatten()));
		}

		/// <summary>
		/// Creates a new <see cref="CodePointCriteria"/> which also excludes code points present in the provided set of <paramref name="codePoints"/>.
		/// </summary>
		public CodePointCriteria Excluding(params string[] codePoints) {
			return Excluding(codePoints.AsEnumerable());
		}

		/// <summary>
		/// Creates a new <see cref="CodePointCriteria"/> which also excludes code points present in the provided set of <paramref name="codePoints"/>.
		/// </summary>
		public CodePointCriteria Excluding(params IEnumerable<string>[] codePoints) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates a new <see cref="CodePointCriteria"/> which also excludes code points present in the provided set of <paramref name="codePoints"/>.
		/// </summary>
		public CodePointCriteria Excluding(params int[] codePoints) {
			return Excluding(codePoints.AsEnumerable());
		}

		/// <summary>
		/// Creates a new <see cref="CodePointCriteria"/> which also excludes code points present in the provided set of <paramref name="codePoints"/>.
		/// </summary>
		public CodePointCriteria Excluding(params IEnumerable<int>[] codePoints) {
			return CloneExcluding(new CodePointInValuesCriterion(codePoints.Flatten()));
		}
		
		/// <summary>
		/// Creates a new <see cref="CodePointCriteria"/> which also excludes code points in the specified range of code points.
		/// </summary>
		/// <param name="start">The start of the range of code points to be excluded, inclusive.</param>
		/// <param name="end">The end of the range of code points to be excluded, inclusive.</param>
		public CodePointCriteria ExcludingRange(char start, char end) {
			return ExcludingRange((int)start, (int)end);
		}

		/// <summary>
		/// Creates a new <see cref="CodePointCriteria"/> which also excludes code points in the specified range of code points.
		/// </summary>
		/// <param name="start">The start of the range of code points to be excluded, inclusive.</param>
		/// <param name="end">The end of the range of code points to be excluded, inclusive.</param>
		public CodePointCriteria ExcludingRange(int start, int end) {
			return CloneExcluding(
				start < end
				? new CodePointInRangeCriterion(start, end)
				: new CodePointInRangeCriterion(end, start)
			);
		}
	}
}
