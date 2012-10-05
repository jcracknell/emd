using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public class SourceRange {
		private readonly int _index;
		private readonly int _line;
		private readonly int _lineIndex;
		private readonly int _length;

		public SourceRange(int index, int length, int line, int lineIndex) {
			_index = index;
			_line = line;
			_lineIndex = lineIndex;
			_length = length;
		}

		public SourceRange(SourceLocation sourceLocation, int length)
			: this(sourceLocation.Index, length, sourceLocation.Line, sourceLocation.LineIndex)
		{ }

		public int Index { get { return _index; } }

		public int Line { get { return _line; } }

		public int LineIndex { get { return _lineIndex; } }

		public int Length { get { return _length; } }

		public override int GetHashCode() {
			return new HashCodeBuilder()
				.Merge(_index)
				.Merge(_line)
				.Merge(_lineIndex)
				.Merge(_length)
				.GetHashCode();
		}

		public override bool Equals(object obj) {
			if(this == obj) return true;

			var other = obj as SourceRange;
			return null != other
				&& this.Index.Equals(other.Index)
				&& this.Length.Equals(other.Length)
				&& this.Line.Equals(other.Line)
				&& this.LineIndex.Equals(other.LineIndex);
		}

		public override string ToString() {
			return string.Concat("{ Index: ", _index, ", Length: ", _length, ", Line: ", _line, ", LineIndex: ", _lineIndex, " }");
		}
	}
}
