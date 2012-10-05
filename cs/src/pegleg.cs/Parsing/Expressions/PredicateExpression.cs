using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public class PredicateExpression : IExpression<Nil> {
		private Func<bool> _predicate;

		public PredicateExpression(Func<bool> predicate) {
			CodeContract.ArgumentIsNotNull(() => predicate, predicate);

			_predicate = predicate;
		}

		public ExpressionType ExpressionType { get { return ExpressionType.Predicate; } }

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public IExpressionMatchingResult Match(IExpressionMatchingContext context) {
			if(_predicate()) {
				return new SuccessfulExpressionMatchingResult(Nil.Value);
			} else {
				return new UnsuccessfulExpressionMatchingResult();
			}
		}
	}
}
