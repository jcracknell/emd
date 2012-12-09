using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public class CharacterParsingExpression : BaseParsingExpression<Nil> {
		private readonly UnicodeCriteria _criteria;

		public CharacterParsingExpression(UnicodeCriteria criteria) {
			if(null == criteria) throw ExceptionBecause.ArgumentNull(() => criteria);

			_criteria = criteria;
		}

		public override IMatchingResult<Nil> Matches(MatchingContext context) {
			int length;
			if(context.ConsumesUnicodeCriteria(_criteria, out length))
				return SuccessfulMatchingResult.Create(Nil.Value, length);
			return UnsuccessfulMatchingResult.Create(this);
		}

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
