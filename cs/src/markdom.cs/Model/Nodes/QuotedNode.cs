using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public enum QuoteType {
		Double,
		Single
	}

	public class QuotedNode : CompositeNode {
		private readonly QuoteType _quoteType;

		public QuotedNode(QuoteType quoteType, Node[] children, SourceRange sourceRange)
			: base(children, sourceRange)
		{
			_quoteType = quoteType;
		}

		public QuoteType QuoteType { get { return _quoteType; } }

		public override NodeType NodeType { get { return NodeType.Quoted; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}

		public override int GetHashCode() {
			return new HashCodeBuilder()
				.Merge(QuoteType)
				.Merge(SourceRange)
				.MergeAll(Children)
				.GetHashCode();
		}

		public override bool Equals(object obj) {
			var other = obj as QuotedNode;
			return null != other
				&& this.QuoteType.Equals(other.QuoteType)
				&& base.Equals(other);
		}
	}
}
