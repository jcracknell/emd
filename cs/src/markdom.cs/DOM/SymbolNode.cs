using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.DOM {
	public class SymbolNode : Node {
		private readonly string _symbol;

		public SymbolNode(string symbol, SourceRange sourceRange)
			: base(sourceRange)
		{
			_symbol = symbol;
		}

		public string Symbol { get { return _symbol; } }

		public override NodeType NodeType { get { return NodeType.Symbol; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}

		public override string ToString() {
			return new HashCodeBuilder()
				.Merge(Symbol)
				.Merge(SourceRange)
				.ToString();
		}

		public override bool Equals(object obj) {
			var other = obj as SymbolNode;
			return null != other
				&& this.Symbol.Equals(other.Symbol, StringComparison.Ordinal)
				&& this.SourceRange.Equals(other.SourceRange);
		}
	}
}
