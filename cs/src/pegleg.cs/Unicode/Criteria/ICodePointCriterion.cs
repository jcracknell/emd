using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode.Criteria {
	/// <summary>
	/// Interface defining a criterion which can be satisfied by a UTF-32 code point.
	/// </summary>
	public interface ICodePointCriterion {
		/// <summary>
		/// Returns true if this <see cref="ICodePointCriterion"/> is satisfied by the provided UTF-32 <paramref name="codePoint"/>.
		/// </summary>
		bool IsSatisfiedBy(int codePoint);
	}
}
