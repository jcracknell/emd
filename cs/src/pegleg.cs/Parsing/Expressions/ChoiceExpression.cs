using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface ChoiceExpression : IExpression {
		IEnumerable<IExpression> Choices { get; }
	}

	public class ChoiceExpression<TProduct> : ChoiceExpression, IExpression<TProduct> {
		private IExpression[] _choices;
		private Func<IExpressionMatch<object>, TProduct> _matchAction;

		public ChoiceExpression(IExpression[] choices, Func<IExpressionMatch<object>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => choices, choices);
			CodeContract.ArgumentIsValid(() => choices, choices.Length >= 2, "must have length of at least two");
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);
			
			_choices = choices;
			_matchAction = matchAction;
		}

		public ExpressionType ExpressionType { get { return ExpressionType.Choice; } }

		public IEnumerable<IExpression> Choices { get { return _choices; } }

		public IExpressionMatchingResult Match(IExpressionMatchingContext context) {
			var matchBuilder = context.StartMatch();

			foreach(var choice in _choices) {
				var choiceMatchingContext = context.Clone();
				var choiceMatchingResult = choice.Match(choiceMatchingContext);

				if(choiceMatchingResult.Succeeded) {
					context.Assimilate(choiceMatchingContext);

					var product = _matchAction(matchBuilder.CompleteMatch(this, choiceMatchingResult.Product));

					return new SuccessfulExpressionMatchingResult(product);
				}
			}

			return new UnsuccessfulExpressionMatchingResult();
		}

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
