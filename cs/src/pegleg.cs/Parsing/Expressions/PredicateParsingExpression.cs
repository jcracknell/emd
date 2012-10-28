using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public class PredicateParsingExpression : IParsingExpression<Nil> {
		private Func<bool> _predicate;

		public PredicateParsingExpression(Func<bool> predicate) {
			CodeContract.ArgumentIsNotNull(() => predicate, predicate);

			_predicate = predicate;
		}

		public ParsingExpressionKind Kind { get { return ParsingExpressionKind.Predicate; } }

		public T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public IMatchingResult Match(IMatchingContext context) {
			if(_predicate()) {
				return new SuccessfulMatchingResult(Nil.Value);
			} else {
				return new UnsuccessfulMatchingResult();
			}
		}
	}
}
