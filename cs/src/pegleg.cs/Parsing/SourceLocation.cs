using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public class SourceLocation : IComparable<SourceLocation> {
		private readonly int _index;
		private readonly int _line;
		private readonly int _lineIndex;

		public SourceLocation(int index, int line, int lineIndex) {
			CodeContract.ArgumentIsValid(() => index, index >= 0, "must be a non-negative integer");
			CodeContract.ArgumentIsValid(() => line, line > 0, "must be a positive integer");	
			CodeContract.ArgumentIsValid(() => lineIndex, lineIndex >= 0, "must be a non-negative integer");

			_index = index;
			_line = line;
			_lineIndex = lineIndex;
		}

		public int Index { get { return _index; } }

		public int Line { get { return _line; } }

		public int LineIndex { get { return _lineIndex; } }

		public int CompareTo(SourceLocation other) {
			return this.Index.CompareTo(other.Index);
		}

		public override int GetHashCode() {
			return _index;
		}

		public override bool Equals(object obj) {
			if(this == obj) return true;
			var other = obj as SourceLocation;
			return null != other && this.Index == other.Index;
		}

		public override string ToString() {
			return string.Concat("{ Index:", _index, ", Line:", _line, ", LineIndex:", _lineIndex, " }");
		}
	}
}
