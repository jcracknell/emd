using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public class MatchBuilder<TProduct> {
		private readonly MatchingContext _matchingContext;
		private readonly IParsingExpression<TProduct> _expression;
		private readonly int _initialConsumed;
		private readonly int _sourceIndex;
		private readonly int _sourceLine;
		private readonly int _sourceLineIndex;

		public MatchBuilder(MatchingContext context, IParsingExpression<TProduct> expression) {
			_matchingContext = context;
			_expression = expression;
			_initialConsumed = context.Consumed;
			_sourceIndex = context.SourceIndex;
			_sourceLine = context.SourceLine;
			_sourceLineIndex = context.SourceLineIndex;
		}

		public IMatch<TRaw> CompleteMatch<TRaw>(TRaw product) {
			var matchLength = _matchingContext.Consumed - _initialConsumed;
			var matchSourceRange = new SourceRange(_sourceIndex, _matchingContext.SourceIndex - _sourceIndex, _sourceLine, _sourceLineIndex);

			return new Match<TRaw>(_matchingContext, product, _initialConsumed, matchLength, matchSourceRange);
		}
	}
}
