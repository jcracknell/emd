using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class RegexParsingExpression<TProduct> : BaseParsingExpression<TProduct> {
		protected readonly Regex _regex;

		public RegexParsingExpression(Regex regex)
			: base(ParsingExpressionKind.Regex)
		{
			CodeContract.ArgumentIsNotNull(() => regex, regex);

			_regex = regex;
		}

		public Regex Regex { get { return _regex; } }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	} 

	public class NonCapturingRegexParsingExpression : RegexParsingExpression<Nil> {
		public NonCapturingRegexParsingExpression(Regex regex) : base(regex) { }

		protected override IMatchingResult<Nil> MatchesCore(IMatchingContext context) {
			if(context.ConsumesMatching(_regex))
				return SuccessfulMatchingResult.NilProduct;
			return UnsuccessfulMatchingResult.Create(this);
		}
	}

	public class CapturingRegexParsingExpression<TProduct> : RegexParsingExpression<TProduct> {
		private readonly Func<IMatch<Match>, TProduct> _matchAction;

		public CapturingRegexParsingExpression(Regex regex, Func<IMatch<Match>, TProduct> matchAction)
			: base(regex)
		{
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_matchAction = matchAction;
		}

		protected override IMatchingResult<TProduct> MatchesCore(IMatchingContext context) {
			var matchBuilder = context.StartMatch();

			Match regexMatch;
			if(!context.ConsumesMatching(_regex, out regexMatch))
				return UnsuccessfulMatchingResult.Create(this);

			var product = _matchAction(matchBuilder.CompleteMatch(this, regexMatch));
			return SuccessfulMatchingResult.Create(product);
		}
	}
}