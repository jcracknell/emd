using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Grammar {
	public class OrderedListItemLineInfo : BlockLineInfo {
		public readonly EnumeratorInfo Enumerator;

		public OrderedListItemLineInfo(EnumeratorInfo enumerator, IEnumerable<LineInfo> lines, SourceRange sourceRange)
			: base(lines, sourceRange)
		{
			Enumerator = enumerator;			
		}
	}
}
