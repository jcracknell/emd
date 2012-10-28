using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class LineBreakNode : IFormattedInlineNode {
		private readonly SourceRange _sourceRange;
		
		public LineBreakNode(SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);

			_sourceRange = sourceRange;
		}

		public NodeType NodeType { get { return NodeType.LineBreak; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
