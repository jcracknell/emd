using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pegleg.cs.Utils;

namespace pegleg.cs.Unicode.Criteria {
	/// <summary>
	/// Criterion which is satisfied by code points present in a provided set of code points.
	/// </summary>
	public class CodePointInValuesCriterion : ICodePointCriterion {
		// Here we map each possible code point to a bit.
		// Per the UTF standard, the maximum value for a code point is 0x10FFFF, expressible in
		// 21 = 5 * 4 + 1 bits.
		//
		// - The first 7 bits of the codepoint form the level 1 index
		// - The next 10 bits of the codepoint form the level 2 index
		// - The last 5 bits of the codepoint as the bit in the uint value referenced by the level 2 index

		private const int
			L1_MASK = 0x007F, L1_SHIFT = 15,
			L2_MASK = 0x03FF, L2_SHIFT = 5,
			L3_MASK = 0x001F;

		private readonly uint[][] _acceptanceMap;

		/// <summary>
		/// Create a criterion satisfied by code points present in the provided set of <paramref name="codePoints"/>.
		/// </summary>
		/// <param name="codePoints">The set of code points which will satisfy the criterion.</param>
		public CodePointInValuesCriterion(IEnumerable<int> codePoints) {
			if(null == codePoints) throw Xception.Because.ArgumentNull(() => codePoints);
			if(!codePoints.Any()) throw Xception.Because.Argument(() => codePoints, "cannot be empty");

			_acceptanceMap = new uint[L1_MASK + 1][];
			foreach(var codePoint in codePoints) {
				if(!UnicodeUtils.IsValidCodePoint(codePoint))
					throw Xception.Because.Argument(() => codePoints, "contains invalid code point " + codePoint);

				int l1i = (codePoint >> L1_SHIFT) & L1_MASK;
				uint[] l1 = _acceptanceMap[l1i] ?? (_acceptanceMap[l1i] = new uint[L2_MASK + 1]);

				int l2i = (codePoint >> L2_SHIFT) & L2_MASK;
				uint l2 = l1[l2i];

				l1[l2i] = l2 | (1u << (codePoint & L3_MASK));
			}
		}

		public bool IsSatisfiedBy(int codePoint) {
			if(!UnicodeUtils.IsValidCodePoint(codePoint)) return false;

			uint[] l1 = _acceptanceMap[(codePoint >> L1_SHIFT) & L1_MASK];
			if(null == l1) return false;

			uint l2 = l1[(codePoint >> L2_SHIFT) & L2_MASK];

			return 0u != l2
				&& 0u != (l2 & (1u << (codePoint & L3_MASK)));
		}
	}
}
