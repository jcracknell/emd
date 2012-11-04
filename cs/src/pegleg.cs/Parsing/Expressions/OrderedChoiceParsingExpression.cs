using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class OrderedChoiceParsingExpression<TChoice, TProduct> : BaseParsingExpression<TProduct> {
		protected readonly IParsingExpression<TChoice>[] _choices;

		public OrderedChoiceParsingExpression(IParsingExpression<TChoice>[] choices)
			: base(ParsingExpressionKind.OrderedChoice)
		{
			CodeContract.ArgumentIsNotNull(() => choices, choices);
			CodeContract.ArgumentIsValid(() => choices, choices.Length >= 2, "must have length of at least two");

			_choices = choices;
		}

		public IEnumerable<IParsingExpression<TChoice>> Choices { get { return _choices; }  }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}

	public class NonCapturingOrderedChoiceParsingExpression : OrderedChoiceParsingExpression<object, Nil> {
		public NonCapturingOrderedChoiceParsingExpression(IParsingExpression<object>[] choices) : base(choices) { }

		protected override IMatchingResult<Nil> MatchesCore(IMatchingContext context) {
			for(int i = 0; i < _choices.Length; i++) {
				var choice = _choices[i];
				var choiceMatchingContext = context.Clone();
				var choiceMatchingResult = choice.Matches(choiceMatchingContext);

				if(choiceMatchingResult.Succeeded) {
					context.Assimilate(choiceMatchingContext);
					return SuccessfulMatchingResult.NilProduct;
				}
			}

			return UnsuccessfulMatchingResult.Create(this);
		}
	}

	public class NonCapturingOrderedChoiceParsingExpression<TChoice> : OrderedChoiceParsingExpression<TChoice, TChoice> {
		public NonCapturingOrderedChoiceParsingExpression(IParsingExpression<TChoice>[] choices) : base(choices) { }

		protected override IMatchingResult<TChoice> MatchesCore(IMatchingContext context) {
			for(int i = 0; i < _choices.Length; i++) {
				var choice = _choices[i];
				var choiceMatchingContext = context.Clone();
				var choiceMatchingResult = choice.Matches(choiceMatchingContext);

				if(choiceMatchingResult.Succeeded) {
					context.Assimilate(choiceMatchingContext);
					return choiceMatchingResult;
				}
			}

			return UnsuccessfulMatchingResult.Create(this);
		}
	}

	public class CapturingOrderedChoiceParsingExpression<TChoice, TProduct> : OrderedChoiceParsingExpression<TChoice, TProduct> {
		private Func<IMatch<TChoice>, TProduct> _matchAction;

		public CapturingOrderedChoiceParsingExpression(IParsingExpression<TChoice>[] choices, Func<IMatch<TChoice>, TProduct> matchAction)
			: base(choices)
		{
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_matchAction = matchAction;
		}

		protected override IMatchingResult<TProduct> MatchesCore(IMatchingContext context) {
			var matchBuilder = context.StartMatch();

			for(int i = 0; i < _choices.Length; i++) {
				var choice = _choices[i];
				var choiceMatchingContext = context.Clone();
				var choiceMatchingResult = choice.Matches(choiceMatchingContext);

				if(choiceMatchingResult.Succeeded) {
					context.Assimilate(choiceMatchingContext);
					var product = _matchAction(matchBuilder.CompleteMatch(this, (TChoice)choiceMatchingResult.Product));
					return SuccessfulMatchingResult.Create(product);
				}
			}

			return UnsuccessfulMatchingResult.Create(this);
		}
	}
}
