using emd.cs.Nodes;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes {
	public class DocumentNode : INode {
		private readonly IBlockNode[] _content;
		private readonly SourceRange _sourceRange;

		public DocumentNode(IBlockNode[] content, SourceRange sourceRange) {
			if(null == content) throw Xception.Because.ArgumentNull(() => content);

			_content = content;
			_sourceRange = sourceRange;
		}

		public IEnumerable<IBlockNode> Content { get { return _content; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
