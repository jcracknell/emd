using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class DynamicParsingExpression : BaseParsingExpression {
		public DynamicParsingExpression() : base(ParsingExpressionKind.Closure) { }

		public abstract IParsingExpression ConstructExpression();
	}

	public class DynamicParsingExpression<TProduct> : DynamicParsingExpression, IParsingExpression<TProduct> {
		private readonly Func<IParsingExpression<TProduct>> _expression;

		public DynamicParsingExpression(Func<IParsingExpression<TProduct>> expression) {
			CodeContract.ArgumentIsNotNull(() => expression, expression);

			_expression = expression;
		}

		public override IParsingExpression ConstructExpression() {
			return _expression();
		}

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			var closed = _expression();
			return closed.Matches(context);
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
