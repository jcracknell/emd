using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class SymbolNode : IPlainInlineNode {
		private readonly string _symbol;
		private readonly MarkdomSourceRange _sourceRange;

		public SymbolNode(string symbol, MarkdomSourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => symbol, symbol);
			CodeContract.ArgumentIsValid(() => symbol, !string.IsNullOrEmpty(symbol), "cannot be empty");

			_symbol = symbol;
			_sourceRange = sourceRange;
		}

		public string Symbol { get { return _symbol; } }

		public NodeType NodeType { get { return NodeType.Symbol; } }

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
