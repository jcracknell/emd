﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Nodes {
	public class BlockquoteNode : IBlockNode {
		private readonly IBlockNode[] _children;
		private readonly SourceRange _sourceRange;

		public BlockquoteNode(IBlockNode[] children, SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => children, children);
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);
			
			_children = children;
			_sourceRange = sourceRange;
		}

		public IEnumerable<IBlockNode> Children { get { return _children; } }

		public NodeKind Kind { get { return NodeKind.Blockquote; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}