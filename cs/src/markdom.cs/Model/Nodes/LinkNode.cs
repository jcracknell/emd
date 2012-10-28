using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class LinkNode : IRichInlineNode {
		private readonly ReferenceId _referenceId;
		private readonly IInlineNode[] _children;
		private readonly MarkdomSourceRange _sourceRange;

		public LinkNode(IInlineNode[] children, ReferenceId referenceId, MarkdomSourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => children, children);

			_children = children;
			_referenceId = referenceId;
			_sourceRange = sourceRange;
		}

		public NodeKind Kind { get { return NodeKind.Link; } }

		public IEnumerable<IInlineNode> Children { get { return _children; } }

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
