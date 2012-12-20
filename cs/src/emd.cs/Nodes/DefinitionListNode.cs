using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes {
	public class DefinitionListNode : IBlockNode {
		private readonly DefinitionListItemNode[] _items;
		private readonly SourceRange _sourceRange;

		public DefinitionListNode(DefinitionListItemNode[] items, SourceRange sourceRange) {
			if(null == items) throw ExceptionBecause.ArgumentNull(() => items);
			if(null == sourceRange) throw ExceptionBecause.ArgumentNull(() => sourceRange);
			if(!items.Any()) throw ExceptionBecause.Argument(() => items, "cannot be empty");

			_items = items;
			_sourceRange = sourceRange;
		}

		public IEnumerable<DefinitionListItemNode> Items { get { return _items; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
