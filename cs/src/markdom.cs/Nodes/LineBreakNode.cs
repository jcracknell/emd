using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Nodes{
	public class LineBreakNode : IFormattedInlineNode {
		private readonly SourceRange _sourceRange;
		
		public LineBreakNode(SourceRange sourceRange) {
			_sourceRange = sourceRange;
		}

		public NodeKind Kind { get { return NodeKind.LineBreak; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
