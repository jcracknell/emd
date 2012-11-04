using markdom.cs.Model.Nodes;
using pegleg.cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Grammar {
	public class EnumeratorCounterStyleDefinition {
		public readonly OrderedListCounterStyle CounterStyle;
		public IParsingExpression<EnumeratorCounterStyleInfo> Expression;

		public EnumeratorCounterStyleDefinition(OrderedListCounterStyle style, IParsingExpression<EnumeratorCounterStyleInfo> expression) {
			CounterStyle = style;
			Expression = expression;
		}
	}
}
