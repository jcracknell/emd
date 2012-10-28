using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class TableRowNode : INode {
		private readonly TableCellNode[] _cells;
		private readonly SourceRange _sourceRange;

		public TableRowNode(TableCellNode[] cells, SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => cells, cells);
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);

			_cells = cells;
			_sourceRange = sourceRange;
		}

		public IEnumerable<TableCellNode> Cells { get { return _cells; } }

		public NodeType NodeType { get { return NodeType.TableRow; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
