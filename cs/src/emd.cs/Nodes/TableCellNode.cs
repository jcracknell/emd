﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes{
	public abstract class TableCellNode : INode {
		private readonly int _rowSpan;
		private readonly int _columnSpan;
		private readonly IInlineNode[] _children;
		private readonly SourceRange _sourceRange;

		public TableCellNode(int columnSpan, int rowSpan, IInlineNode[] children, SourceRange sourceRange)
		{
			if(null == children) throw ExceptionBecause.ArgumentNull(() => children);
			if(!(rowSpan > 0)) throw ExceptionBecause.Argument(() => rowSpan, "must be a positive integer");
			if(!(columnSpan > 0)) throw ExceptionBecause.Argument(() => columnSpan, "must be a positive integer");

			_rowSpan = rowSpan;
			_columnSpan = columnSpan;
			_children = children;
			_sourceRange = sourceRange;
		}

		public int RowSpan { get { return _rowSpan; } }
		
		public int ColumnSpan { get { return _columnSpan; } }

		public IEnumerable<IInlineNode> Children { get { return _children; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public abstract void HandleWith(INodeHandler handler);

		public abstract T HandleWith<T>(INodeHandler<T> handler);
	}
}