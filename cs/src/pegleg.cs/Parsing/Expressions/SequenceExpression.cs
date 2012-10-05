using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface SequenceExpression : IExpression {
		IEnumerable<IExpression> Sequence { get; }
	}

	public class SequenceExpression<TProduct> : SequenceExpression, IExpression<TProduct> {
		private IExpression[] _sequence;
		private readonly Func<IExpressionMatch<object[]>, TProduct> _matchAction;

		public SequenceExpression(IExpression[] sequence, Func<IExpressionMatch<object[]>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => sequence, sequence);
			CodeContract.ArgumentIsValid(() => sequence, sequence.Length >= 2, "must have length of at least two");
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_sequence = sequence;
			_matchAction = matchAction;
		}

		public IExpressionMatchingResult Match(IExpressionMatchingContext context) {
			var matchBuilder = context.StartMatch();

			var expressionProducts = new object[_sequence.Length];
			for(int i = 0; i < _sequence.Length; i++) {
				var currentExpression = _sequence[i];

				var currentExpressionApplicationResult = currentExpression.Match(context);
				
				if(!currentExpressionApplicationResult.Succeeded)
					return currentExpressionApplicationResult;

				expressionProducts[i] = currentExpressionApplicationResult.Product;
			}

			var product = _matchAction(matchBuilder.CompleteMatch(this, expressionProducts));

			return new SuccessfulExpressionMatchingResult(product);
		}

		public IEnumerable<IExpression> Sequence { get { return _sequence; } }

		public ExpressionType ExpressionType { get { return ExpressionType.Sequence; } }

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
