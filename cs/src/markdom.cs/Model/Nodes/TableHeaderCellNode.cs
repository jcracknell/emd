using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class TableHeaderCellNode : TableCellNode {
		public TableHeaderCellNode(int columnSpan, int rowSpan, IInlineNode[] children, MarkdomSourceRange sourceRange)
			: base(columnSpan, rowSpan, children, sourceRange)
		{ }

		public override NodeKind Kind { get { return NodeKind.TableHeaderCell; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
