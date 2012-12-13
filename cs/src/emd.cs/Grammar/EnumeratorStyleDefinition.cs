using emd.cs.Nodes;
using pegleg.cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Grammar {
	public class EnumeratorStyleDefinition {
		public OrderedListSeparatorStyle SeparatorStyle { get; set; }
		public IParsingExpression<object> Preceding { get; set; }
		public IParsingExpression<object> Following { get; set; }
		public IParsingExpression<object> EnumeratorPreamble { get; set; }
		public IParsingExpression<object> EnumeratorPostamble { get; set; }
		public IEnumerable<EnumeratorStyleDefinitionCounter> Counters { get; set; }
	}
}
