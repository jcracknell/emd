using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Expressions {
	public class CallExpression : IExpression {
		private readonly IExpression _body;
		private readonly IExpression[] _arguments;
		private readonly MarkdomSourceRange _sourceRange;

		public CallExpression(IExpression body, IExpression[] arguments, MarkdomSourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => body, body);
			CodeContract.ArgumentIsNotNull(() => arguments, arguments);
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);

			_body = body;
			_arguments = arguments;
			_sourceRange = sourceRange;
		}

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public ExpressionKind Kind { get { return ExpressionKind.Call; } }

		public void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
