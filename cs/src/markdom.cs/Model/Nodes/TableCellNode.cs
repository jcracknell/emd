using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public abstract class TableCellNode : INode {
		private readonly int _rowSpan;
		private readonly int _columnSpan;
		private readonly IInlineNode[] _children;
		private readonly MarkdomSourceRange _sourceRange;

		public TableCellNode(int columnSpan, int rowSpan, IInlineNode[] children, MarkdomSourceRange sourceRange)
		{
			CodeContract.ArgumentIsNotNull(() => children, children);
			CodeContract.ArgumentIsValid(() => rowSpan, rowSpan > 0, "must be a positive integer");
			CodeContract.ArgumentIsValid(() => columnSpan, columnSpan > 0, "must be a positive integer");

			_rowSpan = rowSpan;
			_columnSpan = columnSpan;
			_children = children;
			_sourceRange = sourceRange;
		}

		public int RowSpan { get { return _rowSpan; } }
		
		public int ColumnSpan { get { return _columnSpan; } }

		public IEnumerable<IInlineNode> Children { get { return _children; } }

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public abstract NodeType NodeType { get; }

		public abstract void HandleWith(INodeHandler handler);

		public abstract T HandleWith<T>(INodeHandler<T> handler);
	}
}
