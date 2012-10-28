using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface DynamicParsingExpression : IParsingExpression {
		IParsingExpression ConstructExpression();
	}

	public class DynamicParsingExpression<TProduct> : DynamicParsingExpression, IParsingExpression<TProduct> {
		private readonly Func<IParsingExpression<TProduct>> _expression;

		public DynamicParsingExpression(Func<IParsingExpression<TProduct>> expression) {
			CodeContract.ArgumentIsNotNull(() => expression, expression);

			_expression = expression;
		}

		public IParsingExpression ConstructExpression() {
			return _expression();
		}

		public ParsingExpressionKind Kind { get { return ParsingExpressionKind.Closure; } }

		public IMatchingResult Match(IMatchingContext context) {
			var closed = _expression();
			return closed.Match(context);
		}

		public T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}
	}
}
