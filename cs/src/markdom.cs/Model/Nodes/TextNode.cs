using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Nodes{
	public class TextNode : IPlainInlineNode {
		private readonly string _text;
		private readonly MarkdomSourceRange _sourceRange;

		public TextNode(string text, MarkdomSourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => text, text);
			CodeContract.ArgumentIsValid(() => text, !string.IsNullOrEmpty(text), "cannot be empty");

			_text = text;
			_sourceRange = sourceRange;
		}

		public string Text { get { return _text; } }

		public NodeKind Kind { get { return NodeKind.Text; } }

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}

		public override string ToString() {
			return "(" + _text + ")";
		}
	}
}
