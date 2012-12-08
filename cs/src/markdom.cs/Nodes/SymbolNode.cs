using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Nodes{
	public class SymbolNode : IPlainInlineNode {
		private readonly string _symbol;
		private readonly SourceRange _sourceRange;

		public SymbolNode(string symbol, SourceRange sourceRange) {
			if(null == symbol) throw ExceptionBecause.ArgumentNull(() => symbol);
			if(string.IsNullOrEmpty(symbol)) throw ExceptionBecause.Argument(() => symbol, "cannot be empty");

			_symbol = symbol;
			_sourceRange = sourceRange;
		}

		public string Symbol { get { return _symbol; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
