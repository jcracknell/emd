using emd.cs.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Grammar {
	public class EnumeratorInfo {
		public readonly OrderedListCounterStyle CounterStyle;
		public readonly OrderedListSeparatorStyle SeparatorStyle;
		public readonly int Value;

		public EnumeratorInfo(OrderedListCounterStyle counterStyle, OrderedListSeparatorStyle separatorStyle, int value) {
			CounterStyle = counterStyle;
			SeparatorStyle = separatorStyle;
			Value = value;
		}
	}
}
