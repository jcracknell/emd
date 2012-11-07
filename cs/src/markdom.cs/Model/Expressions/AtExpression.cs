using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Expressions {
	public class AtExpression : IExpression {
		private readonly IExpression _body;
		private readonly MarkdomSourceRange _sourceRange;

		public AtExpression(IExpression body, MarkdomSourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => body, body);

			_body = body;
			_sourceRange = sourceRange;
		}

		public IExpression Body { get { return _body; } }

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public ExpressionKind Kind { get { return ExpressionKind.At; } }

		public void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
