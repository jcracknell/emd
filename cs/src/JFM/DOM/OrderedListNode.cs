using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JFM.DOM {
	public class OrderedListNode : Node {
		private readonly OrderedListStyle _style;
		private readonly int _start;
		private readonly int _increment;
		private readonly OrderedListItemNode[] _items;

		public OrderedListNode(OrderedListStyle style, int start, int increment, OrderedListItemNode[] items, SourceRange sourceRange)
			: base(sourceRange)
		{
			CodeContract.ArgumentIsNotNull(() => items, items);
			CodeContract.ArgumentIsValid(() => items, items.Length >= 1, "cannot be empty");

			_style = style;
			_start = start;
			_increment = increment;
			_items = items;
		}

		public OrderedListStyle Style { get { return _style; } }

		public int Start { get { return _start; } }

		public int Increment { get { return _increment; } }

		public IEnumerable<OrderedListItemNode> Items { get { return _items; } }

		public override NodeType NodeType { get { return NodeType.OrderedList; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
