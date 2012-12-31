using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes{
	public class OrderedListNode : IBlockNode {
		private readonly OrderedListCounterStyle _counterStyle;
		private readonly OrderedListSeparatorStyle _separatorStyle;
		private readonly int _start;
		private readonly OrderedListItemNode[] _items;
		private readonly SourceRange _sourceRange;

		public OrderedListNode(OrderedListCounterStyle counterStyle, OrderedListSeparatorStyle separatorStyle, int start, OrderedListItemNode[] items, SourceRange sourceRange) {
			if(null == items) throw Xception.Because.ArgumentNull(() => items);
			if(!(items.Length >= 1)) throw Xception.Because.Argument(() => items, "cannot be empty");

			_counterStyle = counterStyle;
			_separatorStyle = separatorStyle;
			_start = start;
			_items = items;
			_sourceRange = sourceRange;
		}

		public OrderedListCounterStyle CounterStyle { get { return _counterStyle; } }

		public OrderedListSeparatorStyle SeparatorStyle { get { return _separatorStyle; } }

		public int Start { get { return _start; } }

		public IEnumerable<OrderedListItemNode> Items { get { return _items; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
