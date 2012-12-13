using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Expressions {
	public class NumericLiteralExpression : IExpression {
		private readonly double _value;
		private readonly SourceRange _sourceRange;

		public NumericLiteralExpression(double value, SourceRange sourceRange) {
			_value = value;
			_sourceRange = sourceRange;
		}

		public double Value { get { return _value; } }

		public SourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
