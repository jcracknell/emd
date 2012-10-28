using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class OrderedListItemNode : INode {
		private readonly INode[] _children;
		private readonly MarkdomSourceRange _sourceRange;

		public OrderedListItemNode(INode[] children, MarkdomSourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => children, children);	

			_children = children;
			_sourceRange = sourceRange;
		}

		public IEnumerable<INode> Children { get { return _children; } }

		public NodeKind Kind { get { return NodeKind.OrderedListItem; } }

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
