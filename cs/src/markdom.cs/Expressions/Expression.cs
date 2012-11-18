using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions {
	public abstract class Expression : IExpression {
		private readonly ExpressionKind _kind;
		private readonly SourceRange _sourceRange;

		public Expression(ExpressionKind kind, SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);

			_kind = kind;
			_sourceRange = sourceRange;
		}

		public SourceRange SourceRange { get { return _sourceRange; } }

		public ExpressionKind Kind { get { return _kind; } }

		public abstract void HandleWith(IExpressionHandler handler);

		public abstract T HandleWith<T>(IExpressionHandler<T> handler);
	}
}
