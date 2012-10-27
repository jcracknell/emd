using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.DOM {
	public class SingleLineCommentNode : Node {
		private readonly string _commentText;

		public SingleLineCommentNode(string commentText, SourceRange sourceRange)
			: base(sourceRange)
		{
			CodeContract.ArgumentIsNotNull(() => commentText, commentText);

			_commentText = commentText;
		}

		public string CommentText { get { return _commentText; } }

		public override NodeType NodeType { get { return NodeType.SingleLineComment; } }

		public override void HandleWith(INodeHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(INodeHandler<T> handler) {
			return handler.Handle(this);
		}

		public override bool Equals(object obj) {
			var other = obj as SingleLineCommentNode;
			return null != other
				&& this.CommentText.Equals(other.CommentText, StringComparison.Ordinal)
				&& base.Equals(other);
		}
	}
}
