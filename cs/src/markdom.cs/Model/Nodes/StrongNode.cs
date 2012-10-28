﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class StrongNode : IFormattedInlineNode {
		private readonly IInlineNode[] _children;
		private readonly MarkdomSourceRange _sourceRange;

		public StrongNode(IInlineNode[] children, MarkdomSourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => children, children);

			_children = children;
			_sourceRange = sourceRange;
		}

		public NodeType NodeType { get { return NodeType.Strong; } }

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
