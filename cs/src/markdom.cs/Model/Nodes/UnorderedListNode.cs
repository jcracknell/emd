﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class UnorderedListNode : IBlockNode {
		private readonly UnorderedListItemNode[] _items;
		private readonly MarkdomSourceRange _sourceRange;

		public UnorderedListNode(UnorderedListItemNode[] items, MarkdomSourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => items, items);

			_items = items;
			_sourceRange = sourceRange;
		}

		public IEnumerable<UnorderedListItemNode> Items { get { return _items; } }

		public NodeType NodeType { get { return NodeType.UnorderedListItem; } }

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
