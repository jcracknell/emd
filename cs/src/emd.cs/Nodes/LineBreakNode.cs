using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes{
	public class LineBreakNode : IPlainInlineNode {
		private readonly SourceRange _sourceRange;
		
		public LineBreakNode(SourceRange sourceRange) {
			_sourceRange = sourceRange;
		}

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
