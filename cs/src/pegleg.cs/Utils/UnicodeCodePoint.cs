using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Utils {
	/// <summary>
	/// Class representing a UTF-16 code point (character).
	/// Can optionally be a surrogate pair consisting of two code units.
	/// </summary>
	public class UnicodeCodePoint {
		private readonly bool _isSurrogatePair;
		private readonly char _first;
		private readonly char _second;

		public UnicodeCodePoint(char first) {
			_isSurrogatePair = false;
			_first = first;
		}

		public UnicodeCodePoint(char first, char second) {
			_isSurrogatePair = true;
			_first = first;
			_second = second;
		}

		/// <summary>
		/// The first code unit in this <see cref="UnicodeCodePoint"/>.
		/// </summary>
		public char FirstCodeUnit { get { return _first; } }

		/// <summary>
		/// The second code unit in this <see cref="UnicodeCodePoint"/>, provided this <see cref="UnicodeCodePoint"/> is a surrogate pair.
		/// </summary>
		public char SecondCodeUnit { get { return _second; } }


		/// <summary>
		/// True if this <see cref="UnicodeCodePoint"/> is a surrogate pair containing a second code unit.
		/// </summary>
		public bool IsSurrogatePair { get { return _isSurrogatePair; } }

		public override int GetHashCode() {
			return _isSurrogatePair ? (_first << 16) | _second : _first;
		}

		public override bool Equals(object obj) {
			var other = obj as UnicodeCodePoint;
			return this._first == other._first
				&& this._isSurrogatePair == other._isSurrogatePair
				&& (!this._isSurrogatePair || (this._second == other._second));
		}
	}
}
