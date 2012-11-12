﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Nodes{
	public class ParagraphNode : IBlockNode {
		private readonly IInlineNode[] _children;
		private readonly SourceRange _sourceRange;

		public ParagraphNode(IInlineNode[] children, SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => children, children);
			
			_children = children;
			_sourceRange = sourceRange;
		}

		public IEnumerable<IInlineNode> Children { get { return _children; } }

		public NodeKind Kind { get { return NodeKind.Paragraph; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);	
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}