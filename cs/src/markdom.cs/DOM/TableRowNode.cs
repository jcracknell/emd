using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.DOM {
	public class TableRowNode : Node {
		private readonly TableCellNode[] _cells;

		public TableRowNode(TableCellNode[] cells, SourceRange sourceRange)
			: base(sourceRange)
		{
			CodeContract.ArgumentIsNotNull(() => cells, cells);

			_cells = cells;
		}

		public IEnumerable<TableCellNode> Cells { get { return _cells; } }

		public override NodeType NodeType { get { return NodeType.TableRow; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}

		public override bool Equals(object obj) {
			var other = obj as TableRowNode;
			return null != other
				&& base.Equals(other)
				&& Enumerable.SequenceEqual(this.Cells, other.Cells);
		}
	}
}
