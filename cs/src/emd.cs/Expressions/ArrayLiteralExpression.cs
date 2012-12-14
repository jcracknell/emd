using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Expressions {
	public class ArrayLiteralExpression : IExpression {
		private IExpression[] _elements;
		private SourceRange _sourceRange;

		public ArrayLiteralExpression(IExpression[] elements, SourceRange sourceRange) {
			if(null == elements) throw ExceptionBecause.ArgumentNull(() => elements);
			if(null == sourceRange) throw ExceptionBecause.ArgumentNull(() => sourceRange);
			if(elements.Any(element => null == element)) throw ExceptionBecause.Argument(() => elements, "contains null entries");

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
