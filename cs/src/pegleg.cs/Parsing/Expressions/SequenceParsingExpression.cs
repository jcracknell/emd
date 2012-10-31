using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class SequenceParsingExpression : BaseParsingExpression {
		public SequenceParsingExpression() : base(ParsingExpressionKind.OrderedChoice) { }

		public abstract IEnumerable<IParsingExpression> Sequence { get; }
	}

	public class SequenceParsingExpression<TProduct> : SequenceParsingExpression, IParsingExpression<TProduct> {
		private IParsingExpression[] _sequence;
		private readonly Func<IMatch<SequenceProducts>, TProduct> _matchAction;

		public SequenceParsingExpression(IParsingExpression[] sequence, Func<IMatch<SequenceProducts>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => sequence, sequence);
			CodeContract.ArgumentIsValid(() => sequence, sequence.Length >= 2, "must have length of at least two");
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_sequence = sequence;
			_matchAction = matchAction;
		}

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			var matchBuilder = context.StartMatch();

			var expressionProducts = new object[_sequence.Length];
			for(int i = 0; i < _sequence.Length; i++) {
				var currentExpression = _sequence[i];

				var currentExpressionApplicationResult = currentExpression.Matches(context);
				
				if(!currentExpressionApplicationResult.Succeeded)
					return currentExpressionApplicationResult;

				expressionProducts[i] = currentExpressionApplicationResult.Product;
			}

			var product = _matchAction(matchBuilder.CompleteMatch(this, new SequenceProducts(expressionProducts)));

			return new SuccessfulMatchingResult(product);
		}

		public override IEnumerable<IParsingExpression> Sequence { get { return _sequence; } }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
