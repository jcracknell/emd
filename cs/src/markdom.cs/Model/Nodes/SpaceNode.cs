using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class SpaceNode : IPlainInlineNode {
		private readonly MarkdomSourceRange _sourceRange;

		public SpaceNode(MarkdomSourceRange sourceRange) {
			_sourceRange = sourceRange;
		}

		public NodeKind Kind { get { return NodeKind.Space; } }

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
