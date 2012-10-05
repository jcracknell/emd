using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JFM.DOM {
	public class TextNode : Node {
		private readonly string _text;

		public TextNode(string text, SourceRange sourceRange)
			: base(sourceRange)
		{
			CodeContract.ArgumentIsNotNull(() => text, text);

			_text = text;
		}

		public string Text { get { return _text; } }

		public override NodeType NodeType { get { return NodeType.Text; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}

		public override int GetHashCode() {
			return new HashCodeBuilder()
				.Merge(SourceRange)
				.Merge(Text)
				.GetHashCode();
		}

		public override bool Equals(object obj) {
			if(this == obj) return true;

			var other = obj as TextNode;
			return null != other
				&& this.SourceRange.Equals(other.SourceRange)
				&& this.Text.Equals(other.Text, StringComparison.Ordinal);
		}
	}
}
