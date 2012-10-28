using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface WildcardParsingExpression : IParsingExpression {
	}

	public class WildcardParsingExpression<TProduct> : WildcardParsingExpression, IParsingExpression<TProduct> {
		private readonly Func<IExpressionMatch<string>, TProduct> _matchAction = null;

		public WildcardParsingExpression(Func<IExpressionMatch<string>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_matchAction = matchAction;			
		}

		public ParsingExpressionKind Kind { get { return ParsingExpressionKind.Wildcard; } }

		public T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public IMatchingResult Match(IMatchingContext context) {
			if(context.AtEndOfInput)
				return new UnsuccessfulMatchingResult();

			char c;
			if(null != _matchAction) {
				var matchBuilder = context.StartMatch();

				if(!context.TryConsumeAnyCharacter(out c))
					return new UnsuccessfulMatchingResult();

				var product = _matchAction(matchBuilder.CompleteMatch(this, c.ToString()));

				return new SuccessfulMatchingResult(product);
			} else {
				if(!context.TryConsumeAnyCharacter(out c))
					return new UnsuccessfulMatchingResult();

				return new SuccessfulMatchingResult(c.ToString());
			}
		}

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}
	}
}
