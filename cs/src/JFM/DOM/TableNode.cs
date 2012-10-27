using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JFM.DOM {
	public class TableNode : Node {
		private readonly TableRowNode[] _rows;

		public TableNode(TableRowNode[] rows, SourceRange sourceRange)
			: base(sourceRange)
		{
			CodeContract.ArgumentIsNotNull(() => rows, rows);
			CodeContract.ArgumentIsValid(() => rows, rows.Length > 0, "cannot be empty");

			_rows = rows;
		}

		public IEnumerable<TableRowNode> Rows { get { return _rows; } }

		public override NodeType NodeType { get { return NodeType.Table; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}

		public override bool Equals(object obj) {
			var other = obj as TableNode;
			return null != other
				&& base.Equals(other)
				&& Enumerable.SequenceEqual(this.Rows, other.Rows);
		}
	}
}
