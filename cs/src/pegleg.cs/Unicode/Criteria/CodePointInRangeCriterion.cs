using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode.Criteria {
	/// <summary>
	/// Criterion which is satisfied by a code points in a specified range.
	/// </summary>
	public class CodePointInRangeCriterion : ICodePointCriterion {
		private readonly int _start;
		private readonly int _end;

		/// <summary>
		/// Create a criterion which is satisfied by code points in the specified range.
		/// </summary>
		/// <param name="start">The start of the range of code points satisfying this criterion (inclusive).</param>
		/// <param name="end">The end of the range of code points satisfying this criterion (inclusive).</param>
		public CodePointInRangeCriterion(int start, int end) {
			if(start > end) throw Xception.Because.Argument(() => start, "is invalid");

			_start = start;
			_end = end;
		}

		public bool IsSatisfiedBy(int codePoint) {
			return _start <= codePoint && codePoint <= _end;
		}
	}
}
