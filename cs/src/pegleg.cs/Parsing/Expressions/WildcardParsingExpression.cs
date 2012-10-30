using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class WildcardParsingExpression : BaseParsingExpression {
		public WildcardParsingExpression() : base(ParsingExpressionKind.Wildcard) { }
	}

	public class WildcardParsingExpression<TProduct> : WildcardParsingExpression, IParsingExpression<TProduct> {
		private readonly Func<IExpressionMatch<string>, TProduct> _matchAction = null;

		public WildcardParsingExpression(Func<IExpressionMatch<string>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_matchAction = matchAction;			
		}

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			if(context.AtEndOfInput)
				return new UnsuccessfulMatchingResult();

			char c;
			if(null != _matchAction) {
				var matchBuilder = context.StartMatch();

				if(!context.ConsumesAnyCharacter(out c))
					return new UnsuccessfulMatchingResult();

				var product = _matchAction(matchBuilder.CompleteMatch(this, c.ToString()));

				return new SuccessfulMatchingResult(product);
			} else {
				if(!context.ConsumesAnyCharacter(out c))
					return new UnsuccessfulMatchingResult();

				return new SuccessfulMatchingResult(c.ToString());
			}
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
