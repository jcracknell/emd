using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Expressions {
	public class NullLiteralExpression : IExpression {
		private readonly MarkdomSourceRange _sourceRange;

		public NullLiteralExpression(MarkdomSourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);

			_sourceRange = sourceRange;
		}

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public ExpressionKind Kind { get { return ExpressionKind.NullLiteral; } }

		public void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
