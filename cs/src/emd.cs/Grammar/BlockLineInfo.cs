using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Grammar {
	public class BlockLineInfo {
		public readonly IEnumerable<LineInfo> Lines;
		public readonly SourceRange SourceRange;

		public BlockLineInfo(IEnumerable<LineInfo> lines, SourceRange sourceRange) {
			Lines = lines;
			SourceRange = sourceRange;
		}

		public static BlockLineInfo FromMatch(IMatch<IEnumerable<LineInfo>> match) {
			return new BlockLineInfo(match.Product, match.SourceRange);
		}

		public static BlockLineInfo FromMatch(IMatch<LineInfo> match) {
			return new BlockLineInfo(new LineInfo[] { match.Product }, match.SourceRange);
		}
	}
}
