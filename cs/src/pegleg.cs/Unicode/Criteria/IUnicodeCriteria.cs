using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Unicode.Criteria {
	/// <summary>
	/// Interface defining a set of criteria which can be satisfied by a unicode text element.
	/// </summary>
	public interface IUnicodeCriteria {
		/// <summary>
		/// Returns true if this <see cref="IUnicodeCriteria"/> is satisfied by a unicode text element at 
		/// <paramref name="index"/> in <paramref name="str"/>.
		/// If successful, the length of the satisfying text element is assigned to <paramref name="length"/>.
		/// </summary>
		bool AreSatisfiedBy(string str, int index, out int length);
	}
}
