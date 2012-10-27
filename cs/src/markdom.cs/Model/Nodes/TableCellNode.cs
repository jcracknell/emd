using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public abstract class TableCellNode : CompositeNode {
		private readonly int _rowSpan;
		private readonly int _columnSpan;

		public TableCellNode(int columnSpan, int rowSpan, Node[] children, SourceRange sourceRange)
			: base(children, sourceRange)
		{
			CodeContract.ArgumentIsValid(() => rowSpan, rowSpan > 0, "must be a positive integer");
			CodeContract.ArgumentIsValid(() => columnSpan, columnSpan > 0, "must be a positive integer");

			_rowSpan = rowSpan;
			_columnSpan = columnSpan;
		}

		public int RowSpan { get { return _rowSpan; } }
		
		public int ColumnSpan { get { return _columnSpan; } }
	}
}
