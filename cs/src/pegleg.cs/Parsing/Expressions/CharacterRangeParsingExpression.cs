using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public class CharacterRangeParsingExpression : BaseParsingExpression<Nil> {
		protected readonly char _rangeStart;
		protected readonly char _rangeEnd;

		public CharacterRangeParsingExpression(char rangeStart, char rangeEnd)
			: base(ParsingExpressionKind.CharacterRange)
		{
			if(_rangeEnd < _rangeStart) {
				_rangeStart = rangeEnd;
				_rangeEnd = rangeStart;
			} else {
				_rangeStart = rangeStart;
				_rangeEnd = rangeEnd;
			}
		}

		protected override IMatchingResult<Nil> MatchesCore(IMatchingContext context) {
			if(context.ConsumesCharInRange(_rangeStart, _rangeEnd)) {
				return SuccessfulMatchingResult.NilProduct;
			} else {
				return UnsuccessfulMatchingResult.Create(this);
			}
		}

		public char RangeStart { get { return _rangeStart; } }
		public char RangeEnd { get { return _rangeEnd; } }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
