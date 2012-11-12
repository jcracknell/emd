using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Expressions {
	public class DocumentLiteralExpression : IExpression {
		private readonly INode[] _content;
		private readonly SourceRange _sourceRange;

		public DocumentLiteralExpression(INode[] content, SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => content, content);

			_content = content;
		}

		public IEnumerable<INode> Content { get { return _content; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public ExpressionKind Kind { get { return ExpressionKind.DocumentLiteral; } }

		public void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
