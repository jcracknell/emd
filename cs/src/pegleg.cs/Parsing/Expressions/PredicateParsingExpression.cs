using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public class PredicateParsingExpression : BaseParsingExpression<Nil> {
		private Func<bool> _predicate;

		public PredicateParsingExpression(Func<bool> predicate) {
			if(null == predicate) throw ExceptionBecause.ArgumentNull(() => predicate);

			_predicate = predicate;
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public override IMatchingResult<Nil> Matches(MatchingContext context) {
			if(_predicate()) {
				return SuccessfulMatchingResult.Create(Nil.Value, 0);
			} else {
				return UnsuccessfulMatchingResult.Create(this);
			}
		}
	}
}
