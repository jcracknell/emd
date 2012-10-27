using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.DOM {
	public class UnorderedListItemNode : CompositeNode {
		public UnorderedListItemNode(Node[] children, SourceRange sourceRange)
			: base(children, sourceRange)
		{ }

		public override NodeType NodeType { get { return NodeType.UnorderedListItem; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}

		public override bool Equals(object obj) {
			var other = obj as UnorderedListItemNode;
			return null != other
				&& base.Equals(other);
		}
	}
}
