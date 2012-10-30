using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class CharacterRangeParsingExpression : BaseParsingExpression {
		public CharacterRangeParsingExpression() : base(ParsingExpressionKind.CharacterRange) { }

		public abstract char RangeStart { get; }
		public abstract char RangeEnd { get; }
	}

	public class CharacterRangeParsingExpression<TProduct> : CharacterRangeParsingExpression, IParsingExpression<TProduct> {
		private readonly char _rangeStart;
		private readonly char _rangeEnd;
		private readonly Func<IExpressionMatch<string>, TProduct> _matchAction;

		public CharacterRangeParsingExpression(char rangeStart, char rangeEnd, Func<IExpressionMatch<string>, TProduct> matchAction) {
			// Automatically fix incorrect order
			if(rangeEnd < rangeStart) {
				_rangeStart = rangeEnd;
				_rangeEnd = rangeStart;
			} else {
				_rangeStart = rangeStart;
				_rangeEnd = rangeEnd;
			}

			_matchAction = matchAction;
		}

		public override char RangeStart { get { return _rangeStart; } }

		public override char RangeEnd { get { return _rangeEnd; } }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			char matched;
			if(null == _matchAction) {
				if(!context.ConsumesMatchingCharInRange(_rangeStart, _rangeEnd, out matched))
					return new UnsuccessfulMatchingResult();
				return new SuccessfulMatchingResult(matched.ToString());
			} else {
				var matchBuilder = context.StartMatch();

				if(!context.ConsumesMatchingCharInRange(_rangeStart, _rangeEnd, out matched))
					return new UnsuccessfulMatchingResult();

				var product = _matchAction(matchBuilder.CompleteMatch(this, matched.ToString()));
				return new SuccessfulMatchingResult(product);
			}
		}
	}
}
