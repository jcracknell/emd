using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Expressions {
	public class IndexerExpression : IExpression {
		private readonly IExpression _body;
		private readonly IExpression _name;
		private readonly MarkdomSourceRange _sourceRange;

		public IndexerExpression(IExpression body, IExpression name, MarkdomSourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => body, body);
			CodeContract.ArgumentIsNotNull(() => name, name);
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);

			_body = body;
			_name = name;
			_sourceRange = sourceRange;
		}

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public ExpressionKind Kind { get { return ExpressionKind.Indexer; } }

		public void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
