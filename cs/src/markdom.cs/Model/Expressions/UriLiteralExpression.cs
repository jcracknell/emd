using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Expressions {
	public class UriLiteralExpression : IExpression {
		private readonly string _value;
		private readonly SourceRange _sourceRange;

		public UriLiteralExpression(string value, SourceRange sourceRange) {
			_value = value;
			_sourceRange = sourceRange;
		}

		public string Value { get { return _value; } }

		public ExpressionKind Kind { get { return ExpressionKind.UriLiteral; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
