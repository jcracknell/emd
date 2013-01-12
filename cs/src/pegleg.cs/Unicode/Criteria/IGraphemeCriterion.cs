using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode.Criteria {
	/// <summary>
	/// Interface defining a criterion which can be satisfied by a unicode grapheme.
	/// </summary>
	public interface IGraphemeCriterion {
		/// <summary>
		/// Returns true if this <see cref="IGraphemeCriterion"/> is satisfied by the grapheme
		/// with the provided <paramref name="length"/> and <paramref name="category"/> at
		/// <paramref name="index"/> in <paramref name="str"/>.
		/// Implementations operate under the assumption that all provided arguments are correct.
		/// </summary>
		/// <param name="str">The string containing the candidate grapheme.</param>
		/// <param name="index">The position of the candidate grapheme in <paramref name="str"/>.</param>
		/// <param name="length">The length of the candidate grapheme.</param>
		/// <param name="category">The <see cref="UnicodeCategory"/> of the candidate grapheme.</param>
		bool IsSatisfiedBy(string str, int index, int length, UnicodeCategory category);
	}
}
