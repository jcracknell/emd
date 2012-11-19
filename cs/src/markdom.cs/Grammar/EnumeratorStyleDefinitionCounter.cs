using markdom.cs.Nodes;
using pegleg.cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Grammar {
	public class EnumeratorStyleDefinitionCounter {
		public OrderedListCounterStyle CounterStyle { get; set; }
		public IParsingExpression<EnumeratorCounterStyleInfo> Expression { get; set; }
		public IParsingExpression<EnumeratorInfo> InitialEnumerator { get; set; }
		public IParsingExpression<Nil> ContinuationEnumerator { get; set; }
	}
}
