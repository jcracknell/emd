using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Utils {
	/// <summary>
	/// Class representing an inclusive range of characters.
	/// </summary>
	public class CharacterRange {
		public CharacterRange(char start, char end) {
			Start = start;
			End = end;
		}

		/// <summary>
		/// The character at the start of the range.
		/// </summary>
		public char Start { get; private set; }

		/// <summary>
		/// The Character at the end of the range.
		/// </summary>
		public char End { get; private set; }

		public override int GetHashCode() { return Start ^ End; }

		public override bool Equals(object obj) {
			var other = obj as CharacterRange;
			return null != other && this.Start == other.Start && this.End == other.End;
		}
	}
}
