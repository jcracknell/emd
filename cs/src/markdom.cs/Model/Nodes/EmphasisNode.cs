using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class EmphasisNode : CompositeNode {
		public EmphasisNode(Node[] children, SourceRange sourceRange)
			: base(children, sourceRange)
		{ }

		public override NodeType NodeType { get { return NodeType.Emphasis; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}

		public override bool Equals(object obj) {
			var other = obj as EmphasisNode;
			return base.Equals(other);
		}
	}
}
