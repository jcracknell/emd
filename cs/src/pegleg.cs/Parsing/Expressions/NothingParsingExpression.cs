using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface NothingParsingExpression : IParsingExpression {
	}

	public class NothingParsingExpression<TProduct> : NothingParsingExpression, IParsingExpression<TProduct> {
		private readonly Func<IExpressionMatch<Nil>, TProduct> _matchAction;

		public NothingParsingExpression(Func<IExpressionMatch<Nil>, TProduct> matchAction)
		{
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_matchAction = matchAction;
		}

		public IMatchingResult Match(IMatchingContext context) {
			var product = _matchAction(context.StartMatch().CompleteMatch(this, Nil.Value));

			return new SuccessfulMatchingResult(product);
		}

		public ParsingExpressionKind Kind { get { return ParsingExpressionKind.Nothing; } }

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}

		public T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
