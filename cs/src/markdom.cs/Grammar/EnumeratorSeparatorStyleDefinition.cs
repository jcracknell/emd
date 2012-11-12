using markdom.cs.Nodes;
using pegleg.cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Grammar {
	public class EnumeratorSeparatorStyleDefinition {
		public readonly OrderedListSeparatorStyle SeparatorStyle;
		public readonly IParsingExpression<object> Preceding;
		public readonly IParsingExpression<object> Following;
		public EnumeratorSeparatorStyleDefinition(OrderedListSeparatorStyle style, IParsingExpression<object> preceding, IParsingExpression<object> following) {
			SeparatorStyle = style;
			Preceding = preceding;
			Following = following;
		}
	}
}
