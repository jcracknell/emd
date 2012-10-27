using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class ParagraphNode : CompositeNode {

		public ParagraphNode(Node[] children, SourceRange sourceRange)
			: base(children, sourceRange)
		{ }

		public override NodeType NodeType { get { return NodeType.Paragraph; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);	
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}

		public override int GetHashCode() {
			return new HashCodeBuilder()
				.Merge(SourceRange)
				.MergeAll(Children)
				.GetHashCode();
		}

		public override bool Equals(object obj) {
			if(this == obj) return true;

			var other = obj as ParagraphNode;
			return null != other && this.SourceRange.Equals(other.SourceRange)
				&& Enumerable.SequenceEqual(this.Children, other.Children);
		}
	}
}
