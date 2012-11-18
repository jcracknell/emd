using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions {
	public class DeleteExpression : UnaryExpression {
		private readonly IExpression _body;
		private readonly SourceRange _sourceRange;

		public DeleteExpression(IExpression body, SourceRange sourceRange)
			: base(body, sourceRange) { }

		public override void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
