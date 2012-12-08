using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Nodes{
	public enum QuoteType {
		Double,
		Single
	}

	public class QuotedNode : IPlainInlineNode {
		private readonly QuoteType _quoteType;
		private readonly IInlineNode[] _children;
		private readonly SourceRange _sourceRange;

		public QuotedNode(QuoteType quoteType, IInlineNode[] children, SourceRange sourceRange) {
			if(null == children) throw ExceptionBecause.ArgumentNull(() => children);

			_quoteType = quoteType;
			_children = children;
		}

		public QuoteType QuoteType { get { return _quoteType; } }

		public IEnumerable<IInlineNode> Children { get { return _children; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
