using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface OrderedChoiceParsingExpression : IParsingExpression {
		IEnumerable<IParsingExpression> Choices { get; }
	}

	public class OrderedChoiceParsingExpression<TProduct> : OrderedChoiceParsingExpression, IParsingExpression<TProduct> {
		private IParsingExpression[] _choices;
		private Func<IExpressionMatch<object>, TProduct> _matchAction;

		public OrderedChoiceParsingExpression(IParsingExpression[] choices, Func<IExpressionMatch<object>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => choices, choices);
			CodeContract.ArgumentIsValid(() => choices, choices.Length >= 2, "must have length of at least two");
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);
			
			_choices = choices;
			_matchAction = matchAction;
		}

		public ParsingExpressionKind Kind { get { return ParsingExpressionKind.OrderedChoice; } }

		public IEnumerable<IParsingExpression> Choices { get { return _choices; } }

		public IMatchingResult Match(IMatchingContext context) {
			var matchBuilder = context.StartMatch();

			foreach(var choice in _choices) {
				var choiceMatchingContext = context.Clone();
				var choiceMatchingResult = choice.Match(choiceMatchingContext);

				if(choiceMatchingResult.Succeeded) {
					context.Assimilate(choiceMatchingContext);

					var product = _matchAction(matchBuilder.CompleteMatch(this, choiceMatchingResult.Product));

					return new SuccessfulMatchingResult(product);
				}
			}

			return new UnsuccessfulMatchingResult();
		}

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}

		public T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
