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

		public override IMatchingResult<Nil> Matches(MatchingContext context) {
			for(int i = 0; i < _choices.Length; i++) {
				var choice = _choices[i];
				var choiceMatchingResult = choice.Matches(context);

				if(choiceMatchingResult.Succeeded)
					return SuccessfulMatchingResult.NilProduct;
			}

			return UnsuccessfulMatchingResult.Create(this);
		}
	}

	public class NonCapturingOrderedChoiceParsingExpression<TChoice> : OrderedChoiceParsingExpression<TChoice, TChoice> {
		public NonCapturingOrderedChoiceParsingExpression(IParsingExpression<TChoice>[] choices) : base(choices) { }

		public override IMatchingResult<TChoice> Matches(MatchingContext context) {
			for(int i = 0; i < _choices.Length; i++) {
				var choice = _choices[i];
				var choiceMatchingResult = choice.Matches(context);

				if(choiceMatchingResult.Succeeded)
					return choiceMatchingResult;
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

		public override IMatchingResult<TProduct> Matches(MatchingContext context) {
			var matchBuilder = context.GetMatchBuilderFor(this);

			for(int i = 0; i < _choices.Length; i++) {
				var choice = _choices[i];
				var choiceMatchingResult = choice.Matches(context);

				if(choiceMatchingResult.Succeeded) {
					var product = _matchAction(matchBuilder.CompleteMatch((TChoice)choiceMatchingResult.Product));
					return SuccessfulMatchingResult.Create(product);
				}
			}

			return UnsuccessfulMatchingResult.Create(this);
		}
	}
}
