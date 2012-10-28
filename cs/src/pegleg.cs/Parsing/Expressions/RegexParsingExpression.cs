using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pegleg.cs.Parsing.Expressions {
	public interface RegexParsingExpression : IParsingExpression {
		Regex Regex { get; }
	} 

	public class RegexParsingExpression<TProduct> : RegexParsingExpression, IParsingExpression<TProduct> {
		private readonly Regex _regex;
		private readonly Func<IExpressionMatch<Match>, TProduct> _matchAction;

		public RegexParsingExpression(Regex regex, Func<IExpressionMatch<Match>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => regex, regex);

			_regex = regex;
			_matchAction = matchAction;
		}

		public IMatchingResult Match(IMatchingContext context) {
			Match regexMatch;
			if(null != _matchAction) {
				var matchBuilder = context.StartMatch();
				
				if(!context.TryConsumeMatching(_regex, out regexMatch))
					return new UnsuccessfulMatchingResult();

				var product = _matchAction(matchBuilder.CompleteMatch(this, regexMatch));

				return new SuccessfulMatchingResult(product);
			} else {
				if(!context.TryConsumeMatching(_regex, out regexMatch))
					return new UnsuccessfulMatchingResult();
				return new SuccessfulMatchingResult(regexMatch);
			}
		}

		public Regex Regex { get { return _regex; } }

		public ParsingExpressionKind Kind { get { return ParsingExpressionKind.Regex; } }

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}

		public T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}