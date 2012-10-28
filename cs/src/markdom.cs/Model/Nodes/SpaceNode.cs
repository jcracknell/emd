using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class SpaceNode : IPlainInlineNode {
		private readonly SourceRange _sourceRange;

		public SpaceNode(SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);

			_sourceRange = sourceRange;
		}

		public NodeType NodeType { get { return NodeType.Space; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
