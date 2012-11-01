using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class OrderedChoiceParsingExpression : BaseParsingExpression {
		protected readonly IParsingExpression[] _choices;

		public OrderedChoiceParsingExpression(IParsingExpression[] choices)
			: base(ParsingExpressionKind.OrderedChoice)
		{
			CodeContract.ArgumentIsNotNull(() => choices, choices);
			CodeContract.ArgumentIsValid(() => choices, choices.Length >= 2, "must have length of at least two");

			_choices = choices;
		}

		public IEnumerable<IParsingExpression> Choices { get { return _choices; }  }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}

	public class NonCapturingOrderedChoiceParsingExpression : OrderedChoiceParsingExpression, IParsingExpression<Nil> {
		public NonCapturingOrderedChoiceParsingExpression(IParsingExpression[] choices) : base(choices) { }

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			for(int i = 0; i < _choices.Length; i++) {
				var choice = _choices[i];
				var choiceMatchingContext = context.Clone();
				var choiceMatchingResult = choice.Matches(choiceMatchingContext);

				if(choiceMatchingResult.Succeeded) {
					context.Assimilate(choiceMatchingContext);
					return SuccessfulMatchingResult.NilProduct;
				}
			}

			return new UnsuccessfulMatchingResult();
		}
	}

	public class NonCapturingOrderedChoiceParsingExpression<TChoice> : OrderedChoiceParsingExpression, IParsingExpression<TChoice> {
		public NonCapturingOrderedChoiceParsingExpression(IParsingExpression[] choices) : base(choices) { }

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			for(int i = 0; i < _choices.Length; i++) {
				var choice = _choices[i];
				var choiceMatchingContext = context.Clone();
				var choiceMatchingResult = choice.Matches(choiceMatchingContext);

				if(choiceMatchingResult.Succeeded) {
					context.Assimilate(choiceMatchingContext);
					return choiceMatchingResult;
				}
			}

			return new UnsuccessfulMatchingResult();
		}
	}

	public class CapturingOrderedChoiceParsingExpression<TChoice, TProduct> : OrderedChoiceParsingExpression, IParsingExpression<TProduct> {
		private Func<IMatch<TChoice>, TProduct> _matchAction;

		public CapturingOrderedChoiceParsingExpression(IParsingExpression[] choices, Func<IMatch<TChoice>, TProduct> matchAction)
			: base(choices)
		{
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_matchAction = matchAction;
		}

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			var matchBuilder = context.StartMatch();

			for(int i = 0; i < _choices.Length; i++) {
				var choice = _choices[i];
				var choiceMatchingContext = context.Clone();
				var choiceMatchingResult = choice.Matches(choiceMatchingContext);

				if(choiceMatchingResult.Succeeded) {
					context.Assimilate(choiceMatchingContext);
					var product = _matchAction(matchBuilder.CompleteMatch(this, (TChoice)choiceMatchingResult.Product));
					return new SuccessfulMatchingResult(product);
				}
			}

			return new UnsuccessfulMatchingResult();
		}
	}
}
