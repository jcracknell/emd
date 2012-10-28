using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class TableDataCellNode : TableCellNode {
		public TableDataCellNode(int columnSpan, int rowSpan, IInlineNode[] children, SourceRange sourceRange)
			: base(columnSpan, rowSpan, children, sourceRange)
		{ }

		public override NodeType NodeType { get { return NodeType.TableDataCell; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
