using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface EndOfInputParsingExpression : IParsingExpression {
	}

	public class EndOfInputParsingExpression<TProduct> : EndOfInputParsingExpression, IParsingExpression<TProduct> {
		private readonly Func<IExpressionMatch<Nil>, TProduct> _matchAction;

		public EndOfInputParsingExpression(Func<IExpressionMatch<Nil>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_matchAction = matchAction;
		}

		public ParsingExpressionKind Kind { get { return ParsingExpressionKind.EndOfInput; } }

		public T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public IMatchingResult Match(IMatchingContext context) {
			if(!context.AtEndOfInput)
				return new UnsuccessfulMatchingResult();

			var product = _matchAction(context.StartMatch().CompleteMatch(this, Nil.Value));

			return new SuccessfulMatchingResult(product);
		}
	}
}
