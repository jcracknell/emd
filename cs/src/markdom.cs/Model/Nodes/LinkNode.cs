﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class LinkNode : IRichInlineNode {
		private readonly ReferenceId _referenceId;
		private readonly IInlineNode[] _children;
		private readonly MarkdomSourceRange _sourceRange;
		private readonly IExpression[] _arguments;

		public LinkNode(IInlineNode[] children, ReferenceId referenceId, IExpression[] arguments, MarkdomSourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => children, children);
			CodeContract.ArgumentIsNotNull(() => arguments, arguments);

			_children = children;
			_referenceId = referenceId;
			_sourceRange = sourceRange;
			_arguments = arguments;
		}

		public NodeKind Kind { get { return NodeKind.Link; } }

		public IEnumerable<IInlineNode> Children { get { return _children; } }

		public IEnumerable<IExpression> Arguments { get { return _arguments; } }

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
