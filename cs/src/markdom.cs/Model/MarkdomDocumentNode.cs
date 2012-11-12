﻿using markdom.cs.Model.Nodes;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model {
	public class MarkdomDocumentNode : INode {
		private readonly IBlockNode[] _content;
		private readonly SourceRange _sourceRange;

		public MarkdomDocumentNode(IBlockNode[] content, SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => content, content);

			_content = content;
			_sourceRange = sourceRange;
		}

		public IEnumerable<IBlockNode> Content { get { return _content; } }

		public NodeKind Kind { get { return NodeKind.Document; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
