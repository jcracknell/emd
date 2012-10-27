using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class StrongNode : CompositeNode {

		public StrongNode(Node[] children, SourceRange sourceRange)
			: base(children, sourceRange)
		{ }

		public override NodeType NodeType { get { return NodeType.Strong; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}

		public override bool Equals(object obj) {
			if(this == obj) return true;

			var other = obj as StrongNode;
			return base.Equals(other);
		}
	}
}
