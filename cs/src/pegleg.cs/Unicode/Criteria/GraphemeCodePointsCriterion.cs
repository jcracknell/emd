using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using pegleg.cs.Utils;

namespace pegleg.cs.Unicode.Criteria {
	/// <summary>
	/// Criterion which is satisfied by graphemes for which all constituent code points satisfy provided
	/// <see cref="CodePointCriteria"/>.
	/// </summary>
	public class GraphemeCodePointsCriterion : IGraphemeCriterion {
		private readonly CodePointCriteria _codePointCriteria;

		/// <summary>
		/// Create a criterion which is satisfied by graphemes for which all constituent code points satisfy the
		/// provided <paramref name="codePointCriteria"/>.
		/// </summary>
		public GraphemeCodePointsCriterion(CodePointCriteria codePointCriteria) {
			if(null == codePointCriteria) throw Xception.Because.ArgumentNull(() => codePointCriteria);

			_codePointCriteria = codePointCriteria;
		}

		public bool IsSatisfiedBy(string str, int index, int length, UnicodeCategory category) {
			var graphemeEnd = index + length;
			while(index < graphemeEnd) {
				int codePointLength;
				if(!_codePointCriteria.AreSatisfiedBy(str, index, out codePointLength))
					return false;

				index += codePointLength;
			}

			return true;
		}
	}
}
