using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public class DynamicParsingExpression<TProduct> : BaseParsingExpression<TProduct> {
		private readonly Func<IParsingExpression<TProduct>> _expression;

		public DynamicParsingExpression(Func<IParsingExpression<TProduct>> expression) {
			if(null == expression) throw Xception.Because.ArgumentNull(() => expression);

			_expression = expression;
		}

		public IParsingExpression ConstructExpression() {
			return _expression();
		}

		public override IMatchingResult<TProduct> Matches(MatchingContext context) {
			var closed = _expression();
			return closed.Matches(context);
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
