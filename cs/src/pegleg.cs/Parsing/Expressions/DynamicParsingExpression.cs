using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public class DynamicParsingExpression<TProduct> : BaseParsingExpression<TProduct> {
		private readonly Func<IParsingExpression<TProduct>> _expression;

		public DynamicParsingExpression(Func<IParsingExpression<TProduct>> expression)
			: base(ParsingExpressionKind.Closure)
		{
			CodeContract.ArgumentIsNotNull(() => expression, expression);

			_expression = expression;
		}

		public IParsingExpression ConstructExpression() {
			return _expression();
		}

		protected override IMatchingResult<TProduct> MatchesCore(MatchingContext context) {
			var closed = _expression();
			return closed.Matches(context);
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
