using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public class MatchBuilder<TProduct> {
		private readonly MatchingContext _matchingContext;
		private readonly IParsingExpression<TProduct> _expression;
		private readonly SourceLocation _initialSourceLocation;
		private readonly int _initialConsumed;

		public MatchBuilder(MatchingContext context, IParsingExpression<TProduct> expression) {
			_matchingContext = context;
			_expression = expression;
			_initialConsumed = context.Consumed;
			_initialSourceLocation = context.SourceLocation;
		}

		public IMatch<TRaw> CompleteMatch<TRaw>(TRaw product) {
			var matchLength = _matchingContext.Consumed - _initialConsumed;
			var matchSourceRange = new SourceRange(
				_initialSourceLocation,
				_matchingContext.SourceLocation.Index - _initialSourceLocation.Index);

			return new Match<TRaw>(_matchingContext, product, _initialConsumed, matchLength, matchSourceRange);
		}
	}
}
