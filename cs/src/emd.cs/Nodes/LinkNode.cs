using emd.cs.Expressions;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Nodes{
	public class LinkNode : IRichInlineNode {
		private readonly ReferenceId _referenceId;
		private readonly IInlineNode[] _children;
		private readonly SourceRange _sourceRange;
		private readonly IExpression[] _arguments;

		public LinkNode(IInlineNode[] children, ReferenceId referenceId, IExpression[] arguments, SourceRange sourceRange) {
			if(null == children) throw Xception.Because.ArgumentNull(() => children);

			_children = children;
			_referenceId = referenceId;
			_sourceRange = sourceRange;
			_arguments = arguments;
		}

		public IEnumerable<IInlineNode> Children { get { return _children; } }

		public ReferenceId ReferenceId { get { return _referenceId; } }

		public IEnumerable<IExpression> Arguments { get { return _arguments; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
