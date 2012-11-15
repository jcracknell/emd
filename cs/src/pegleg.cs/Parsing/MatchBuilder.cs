using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public class MatchBuilder {
		private readonly MatchingContext _matchingContext;
		private readonly IParsingExpression _expression;
		private readonly SourceLocation _initialSourceLocation;
		private readonly int _initialConsumed;

		public MatchBuilder(MatchingContext context, IParsingExpression expression) {
			_matchingContext = context;
			_expression = expression;
			_initialConsumed = context.Consumed;
			_initialSourceLocation = context.SourceLocation;
		}

		public IMatch<TProduct> CompleteMatch<TProduct>(TProduct product) {
			var matchLength = _matchingContext.Consumed - _initialConsumed;
			var matchSourceRange = new SourceRange(
				_initialSourceLocation,
				_matchingContext.SourceLocation.Index - _initialSourceLocation.Index);

			return new Match<TProduct>(_matchingContext, _expression, product, _initialConsumed, matchLength, matchSourceRange);
		}
	}
}
