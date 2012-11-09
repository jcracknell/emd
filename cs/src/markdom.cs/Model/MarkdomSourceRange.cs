﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model {
	public class MarkdomSourceRange {
		public readonly int Index;
		public readonly int Length;
		public readonly int Line;
		public readonly int LineIndex;

		public MarkdomSourceRange(int index, int length, int line, int lineIndex) {
			CodeContract.ArgumentIsValid(() => index, index >= 0, "must be a non-negative integer");
			CodeContract.ArgumentIsValid(() => length, length >= 0, "must be non-negative integer");
			CodeContract.ArgumentIsValid(() => line, line >= 1, "must be a positive integer");
			CodeContract.ArgumentIsValid(() => lineIndex, lineIndex >= 0, "must be a non-negative integer");
			CodeContract.ArgumentIsValid(() => lineIndex, lineIndex <= index, "must be less than index");

			Index = index;
			Length = length;
			Line = line;
			LineIndex = lineIndex;
		}

		public static MarkdomSourceRange FromSource(SourceRange sourceRange) {
			return new MarkdomSourceRange(
				sourceRange.Index,
				sourceRange.Length,
				sourceRange.Line,
				sourceRange.LineIndex);
		}

		public static MarkdomSourceRange FromMatch(IMatch match) {
			return FromSource(match.SourceRange);
		}

		public override bool Equals(object obj) {
			var other = obj as MarkdomSourceRange;
			return null != other
				&& this.Index == other.Index
				&& this.Length == other.Length
				&& this.Line == other.Line
				&& this.LineIndex == other.LineIndex;
		}
	}
}
