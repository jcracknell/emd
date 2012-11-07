using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Expressions {
	public class BooleanLiteralExpression : IExpression {
		private readonly bool _value;
		private readonly MarkdomSourceRange _sourceRange;

		public BooleanLiteralExpression(bool value, MarkdomSourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);

			_value = value;
			_sourceRange = sourceRange;
		}

		public bool Value { get { return _value; } }

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public ExpressionKind Kind { get { return ExpressionKind.BooleanLiteral; } }

		public void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
