using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pegleg.cs.Parsing.Expressions {
	public interface RegexExpression : IExpression {
		Regex Regex { get; }
	} 

	public class RegexExpression<TProduct> : RegexExpression, IExpression<TProduct> {
		private readonly Regex _regex;
		private readonly Func<IExpressionMatch<Match>, TProduct> _matchAction;

		public RegexExpression(Regex regex, Func<IExpressionMatch<Match>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => regex, regex);

			_regex = regex;
			_matchAction = matchAction;
		}

		public IExpressionMatchingResult Match(IExpressionMatchingContext context) {
			Match regexMatch;
			if(null != _matchAction) {
				var matchBuilder = context.StartMatch();
				
				if(!context.TryConsumeMatching(_regex, out regexMatch))
					return new UnsuccessfulExpressionMatchingResult();

				var product = _matchAction(matchBuilder.CompleteMatch(this, regexMatch));

				return new SuccessfulExpressionMatchingResult(product);
			} else {
				if(!context.TryConsumeMatching(_regex, out regexMatch))
					return new UnsuccessfulExpressionMatchingResult();
				return new SuccessfulExpressionMatchingResult(regexMatch);
			}
		}

		public Regex Regex { get { return _regex; } }

		public ExpressionType ExpressionType { get { return ExpressionType.Regex; } }

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}