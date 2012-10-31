using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class EndOfInputParsingExpression : BaseParsingExpression {
		public EndOfInputParsingExpression() : base(ParsingExpressionKind.EndOfInput) { }
	}

	public class EndOfInputParsingExpression<TProduct> : EndOfInputParsingExpression, IParsingExpression<TProduct> {
		private readonly Func<IMatch<Nil>, TProduct> _matchAction;

		public EndOfInputParsingExpression(Func<IMatch<Nil>, TProduct> matchAction) {
			_matchAction = matchAction;
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			if(!context.AtEndOfInput)
				return new UnsuccessfulMatchingResult();

			if(null == _matchAction)
				return new SuccessfulMatchingResult(Nil.Value);

			var product = _matchAction(context.StartMatch().CompleteMatch(this, Nil.Value));
			return new SuccessfulMatchingResult(product);
		}
	}
}
