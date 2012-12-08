using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Nodes{
	public class UnorderedListNode : IBlockNode {
		private readonly UnorderedListItemNode[] _items;
		private readonly SourceRange _sourceRange;

		public UnorderedListNode(UnorderedListItemNode[] items, SourceRange sourceRange) {
			if(null == items) throw ExceptionBecause.ArgumentNull(() => items);

			_items = items;
			_sourceRange = sourceRange;
		}

		public IEnumerable<UnorderedListItemNode> Items { get { return _items; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
