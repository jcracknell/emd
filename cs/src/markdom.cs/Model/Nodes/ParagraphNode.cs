using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class ParagraphNode : IBlockNode {
		private readonly IInlineNode[] _children;
		private readonly SourceRange _sourceRange;

		public ParagraphNode(IInlineNode[] children, SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => children, children);
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);
			
			_children = children;
			_sourceRange = sourceRange;
		}

		public IEnumerable<IInlineNode> Children { get { return _children; } }

		public NodeType NodeType { get { return NodeType.Paragraph; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);	
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
