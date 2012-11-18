using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions {
	public abstract class Expression : IExpression {
		private readonly SourceRange _sourceRange;

		public Expression(SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);

			_sourceRange = sourceRange;
		}

		public SourceRange SourceRange { get { return _sourceRange; } }

		public abstract void HandleWith(IExpressionHandler handler);

		public abstract T HandleWith<T>(IExpressionHandler<T> handler);
	}
}
