using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface CharacterRangeExpression : IExpression {
		char RangeStart { get; }
		char RangeEnd { get; }
	}

	public class CharacterRangeExpression<TProduct> : CharacterRangeExpression, IExpression<TProduct> {
		private readonly char _rangeStart;
		private readonly char _rangeEnd;
		private readonly Func<IExpressionMatch<string>, TProduct> _matchAction;

		public CharacterRangeExpression(char rangeStart, char rangeEnd, Func<IExpressionMatch<string>, TProduct> matchAction) {
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

		public char RangeStart { get { return _rangeStart; } }

		public char RangeEnd { get { return _rangeEnd; } }

		public ExpressionType ExpressionType { get { return ExpressionType.CharacterRange; } }

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public IExpressionMatchingResult Match(IExpressionMatchingContext context) {
			char matched;
			if(null == _matchAction) {
				if(!context.TryConsumeMatchingCharInRange(_rangeStart, _rangeEnd, out matched))
					return new UnsuccessfulExpressionMatchingResult();
				return new SuccessfulExpressionMatchingResult(matched.ToString());
			} else {
				var matchBuilder = context.StartMatch();

				if(!context.TryConsumeMatchingCharInRange(_rangeStart, _rangeEnd, out matched))
					return new UnsuccessfulExpressionMatchingResult();

				var product = _matchAction(matchBuilder.CompleteMatch(this, matched.ToString()));
				return new SuccessfulExpressionMatchingResult(product);
			}
		}

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}
	}
}
