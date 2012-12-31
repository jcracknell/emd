using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes{
	public class TableNode : IBlockNode {
		private readonly TableRowNode[] _rows;
		private readonly SourceRange _sourceRange;

		public TableNode(TableRowNode[] rows, SourceRange sourceRange) {
			if(null == rows) throw Xception.Because.ArgumentNull(() => rows);
			if(!(rows.Length > 0)) throw Xception.Because.Argument(() => rows, "cannot be empty");

			_rows = rows;
			_sourceRange = sourceRange;
		}

		public IEnumerable<TableRowNode> Rows { get { return _rows; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
