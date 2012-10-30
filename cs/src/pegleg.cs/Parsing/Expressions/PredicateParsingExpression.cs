using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public class PredicateParsingExpression : BaseParsingExpression, IParsingExpression<Nil> {
		private Func<bool> _predicate;

		public PredicateParsingExpression(Func<bool> predicate)
			: base(ParsingExpressionKind.Predicate)
		{
			CodeContract.ArgumentIsNotNull(() => predicate, predicate);

			_predicate = predicate;
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			if(_predicate()) {
				return new SuccessfulMatchingResult(Nil.Value);
			} else {
				return new UnsuccessfulMatchingResult();
			}
		}
	}
}
