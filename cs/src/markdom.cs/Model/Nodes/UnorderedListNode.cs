using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class UnorderedListNode : Node {
		private UnorderedListItemNode[] _items;

		public UnorderedListNode(UnorderedListItemNode[] items, SourceRange sourceRange)
			: base(sourceRange)
		{
			CodeContract.ArgumentIsNotNull(() => items, items);

			_items = items;
		}

		public IEnumerable<UnorderedListItemNode> Items { get { return _items; } }

		public override NodeType NodeType { get { return NodeType.UnorderedListItem; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}

		public override bool Equals(object obj) {
			var other = obj as UnorderedListNode;
			return null != other
				&& Enumerable.SequenceEqual(this.Items, other.Items)
				&& base.Equals(other);
		}
	}
}
