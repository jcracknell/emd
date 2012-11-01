using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public abstract class CharacterRangeParsingExpression : BaseParsingExpression {
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

		public char RangeStart { get { return _rangeStart; } }
		public char RangeEnd { get { return _rangeEnd; } }

		public override T HandleWith<T>(IParsingExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}

	public class NonCapturingCharacterRangeParsingExpression : CharacterRangeParsingExpression, IParsingExpression<Nil> {
		public NonCapturingCharacterRangeParsingExpression(char rangeStart, char rangeEnd) : base(rangeStart, rangeEnd) { }

		protected override IMatchingResult MatchesCore(IMatchingContext context) {
			if(context.ConsumesCharInRange(_rangeStart, _rangeEnd)) {
				return SuccessfulMatchingResult.NilProduct;
			} else {
				return new UnsuccessfulMatchingResult();
			}
		}
	}
}
