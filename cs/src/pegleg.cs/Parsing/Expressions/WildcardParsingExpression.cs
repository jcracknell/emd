using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class WildcardParsingExpression : BaseParsingExpression {
		public WildcardParsingExpression() : base(ParsingExpressionKind.Wildcard) { }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}

	public class NonCapturingWildcardParsingExpression : WildcardParsingExpression, IParsingExpression<Nil> {
		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			if(context.AtEndOfInput)
				return new UnsuccessfulMatchingResult();

			if(context.ConsumesAnyCharacter())
				return SuccessfulMatchingResult.NilProduct;

			return new UnsuccessfulMatchingResult();
		}
	}

	public class CapturingWildcardParsingExpression<TProduct> : WildcardParsingExpression, IParsingExpression<TProduct> {
		private readonly Func<IMatch<char>, TProduct> _matchAction = null;

		public CapturingWildcardParsingExpression(Func<IMatch<char>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);
			_matchAction = matchAction;			
		}

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			if(context.AtEndOfInput)
				return new UnsuccessfulMatchingResult();

			var matchBuilder = context.StartMatch();
			char c;
			if(!context.ConsumesAnyCharacter(out c))
				return new UnsuccessfulMatchingResult();

			var product = _matchAction(matchBuilder.CompleteMatch(this, c));
			return new SuccessfulMatchingResult(product);
		}
	}
}
