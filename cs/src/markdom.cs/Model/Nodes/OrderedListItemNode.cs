using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class OrderedListItemNode : CompositeNode {
		public OrderedListItemNode(Node[] children, SourceRange sourceRange)
			: base(children, sourceRange)
		{ }

		public override NodeType NodeType { get { return NodeType.OrderedListItem; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
