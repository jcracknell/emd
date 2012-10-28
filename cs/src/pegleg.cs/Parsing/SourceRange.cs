using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public struct SourceRange {
		public readonly int Index;
		public readonly int Line;
		public readonly int LineIndex;
		public readonly int Length;

		public SourceRange(int index, int length, int line, int lineIndex) {
			CodeContract.ArgumentIsValid(() => index, index >= 0, "must be a non-negative integer");
			CodeContract.ArgumentIsValid(() => length, length >= 0, "must be non-negative integer");
			CodeContract.ArgumentIsValid(() => line, line >= 1, "must be a positive integer");
			CodeContract.ArgumentIsValid(() => lineIndex, lineIndex >= 0, "must be a non-negative integer");
			CodeContract.ArgumentIsValid(() => lineIndex, lineIndex <= index, "must be less than index");

			Index = index;
			Line = line;
			LineIndex = lineIndex;
			Length = length;
		}

		public SourceRange(SourceLocation sourceLocation, int length)
			: this(sourceLocation.Index, length, sourceLocation.Line, sourceLocation.LineIndex)
		{ }

		public override string ToString() {
			return string.Concat("{ Index: ", Index, ", Length: ", Length, ", Line: ", Line, ", LineIndex: ", LineIndex, " }");
		}
	}
}
