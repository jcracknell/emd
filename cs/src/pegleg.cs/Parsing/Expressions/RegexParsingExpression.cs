using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class RegexParsingExpression : BaseParsingExpression {
		public RegexParsingExpression() : base(ParsingExpressionKind.Regex) { }

		public abstract Regex Regex { get; }
	} 

	public class RegexParsingExpression<TProduct> : RegexParsingExpression, IParsingExpression<TProduct> {
		private readonly Regex _regex;
		private readonly Func<IExpressionMatch<Match>, TProduct> _matchAction;

		public RegexParsingExpression(Regex regex, Func<IExpressionMatch<Match>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => regex, regex);

			_regex = regex;
			_matchAction = matchAction;
		}

		public override Regex Regex { get { return _regex; } }

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			Match regexMatch;
			if(null != _matchAction) {
				var matchBuilder = context.StartMatch();
				
				if(!context.ConsumesMatching(_regex, out regexMatch))
					return new UnsuccessfulMatchingResult();

				var product = _matchAction(matchBuilder.CompleteMatch(this, regexMatch));

				return new SuccessfulMatchingResult(product);
			} else {
				if(!context.ConsumesMatching(_regex, out regexMatch))
					return new UnsuccessfulMatchingResult();
				return new SuccessfulMatchingResult(regexMatch);
			}
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return this.HandleWith(handler);
		}
	}
}