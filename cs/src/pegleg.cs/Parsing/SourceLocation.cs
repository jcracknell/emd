using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public struct SourceLocation {
		public readonly int Index;
		public readonly int Line;
		public readonly int LineIndex;

		public SourceLocation(int index, int line, int lineIndex) {
			CodeContract.ArgumentIsValid(() => index, index >= 0, "must be a non-negative integer");
			CodeContract.ArgumentIsValid(() => line, line >= 1, "must be a positive integer");
			CodeContract.ArgumentIsValid(() => lineIndex, lineIndex >= 0, "must be a non-negative integer");
			CodeContract.ArgumentIsValid(() => lineIndex, lineIndex <= index, "must be less than index");

			Index = index;
			Line = line;
			LineIndex = lineIndex;
		}

		public override string ToString() {
			return string.Concat("{ Index:", Index, ", Line:", Line, ", LineIndex:", LineIndex, " }");
		}
	}
}
