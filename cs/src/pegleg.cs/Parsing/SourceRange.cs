using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public class SourceRange {
		public readonly int Index;
		public readonly int Line;
		public readonly int LineIndex;
		public readonly int Length;

		public SourceRange(int index, int length, int line, int lineIndex) {
			if(index < 0) throw Xception.Because.Argument(() => index, "must be a non-negative integer");
			if(length < 0) throw Xception.Because.Argument(() => length, "must be non-negative integer");
			if(line < 1) throw Xception.Because.Argument(() => line, "must be a positive integer");
			if(lineIndex < 0) throw Xception.Because.Argument(() => lineIndex, "must be a non-negative integer");
			if(lineIndex > index) throw Xception.Because.Argument(() => lineIndex, "must be less than index");

			Index = index;
			Line = line;
			LineIndex = lineIndex;
			Length = length;
		}

		/// <summary>
		/// Calculate a new <see cref="SourceRange"/> starting at this value and extending through the provided <see cref="end"/> range.
		/// </summary>
		/// <param name="end">The ending <see cref="SourceRange"/> used to calculate the extent of the resulting <see cref="SourceRange"/>.</param>
		/// <returns>A new <see cref="SourceRange"/> at the current value's position extending throught the provided <paramref name="end"/>.</returns>
		public SourceRange Through(SourceRange end) {
			if(null == end) throw Xception.Because.ArgumentNull(() => end);

			return new SourceRange(Index, end.Index - Index + end.Length, Line, LineIndex);
		}

		/// <summary>
		/// Create a new <see cref="SourceRange"/> at the position of the current value with length 0.
		/// </summary>
		/// <returns>A new <see cref="SourceRange"/> at the position of the current value with length 0.</returns>
		public SourceRange Position() {
			return new SourceRange(Index, 0, Line, LineIndex);
		}

		public override int GetHashCode() {
			return ((Index << 16) | (Index >> 16))
				^ Length
				^ (Line << 16)
				^ (LineIndex << 8);
		}

		public override bool Equals(object obj) {
			var other = obj as SourceRange;
			return null != other
				&& this.Index == other.Index
				&& this.Length == other.Length
				&& this.Line == other.Line
				&& this.LineIndex == other.LineIndex;
		}

		public override string ToString() {
			return string.Concat("{ Index: ", Index, ", Length: ", Length, ", Line: ", Line, ", LineIndex: ", LineIndex, " }");
		}
	}
}
