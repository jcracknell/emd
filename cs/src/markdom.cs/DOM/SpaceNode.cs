using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.DOM {
	public class SpaceNode : Node {
		public SpaceNode(SourceRange sourceRange)
			: base(sourceRange)
		{ }

		public override NodeType NodeType { get { return NodeType.Space; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}

		public override bool Equals(object obj) {
			var other = obj as SpaceNode;
			return null != other && this.SourceRange.Equals(other.SourceRange);
		}
	}
}
