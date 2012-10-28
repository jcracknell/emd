using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Expressions {
	public class NumericLiteralExpression : IExpression {
		private readonly double _value;
		private readonly MarkdomSourceRange _sourceRange;

		public NumericLiteralExpression(double value, MarkdomSourceRange sourceRange) {
			_value = value;
			_sourceRange = sourceRange;
		}

		public double Value { get { return _value; } }

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public ExpressionKind Kind { get { return ExpressionKind.NumericLiteral; } }

		public void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
