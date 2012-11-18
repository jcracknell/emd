using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions {
	public class ArrayLiteralExpression : IExpression {
		private IExpression[] _elements;
		private SourceRange _sourceRange;

		public ArrayLiteralExpression(IExpression[] elements, SourceRange sourceRange) {
			CodeContract.ArgumentIsNotNull(() => elements, elements);
			CodeContract.ArgumentIsNotNull(() => sourceRange, sourceRange);

			_elements = elements;
			_sourceRange = sourceRange;
		}

		public IEnumerable<IExpression> Elements { get { return _elements; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
