﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Nodes {
	public class CodeNode : IInlineNode {
		private readonly string _text;
		private readonly SourceRange _sourceRange;
		
		public CodeNode(string text, SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => text, text);
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);

			_text = text;
			_sourceRange = sourceRange;
		}

		public NodeKind Kind { get { return NodeKind.Code; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}