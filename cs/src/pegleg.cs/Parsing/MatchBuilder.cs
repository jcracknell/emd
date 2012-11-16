using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public class MatchBuilder<TProduct> {
		private readonly MatchingContext _matchingContext;
		private readonly IParsingExpression<TProduct> _expression;
		private readonly int _index;
		private readonly int _sourceIndex;
		private readonly int _sourceLine;
		private readonly int _sourceLineIndex;

		public MatchBuilder(MatchingContext context, IParsingExpression<TProduct> expression) {
			_matchingContext = context;
			_expression = expression;
			_index = context.Index;
			_sourceIndex = context.SourceIndex;
			_sourceLine = context.SourceLine;
			_sourceLineIndex = context.SourceLineIndex;
		}

		public IMatch<TRaw> CompleteMatch<TRaw>(TRaw product) {
			var matchLength = _matchingContext.Index - _index;
			var matchSourceRange = new SourceRange(_sourceIndex, _matchingContext.SourceIndex - _sourceIndex, _sourceLine, _sourceLineIndex);
			var sourceLength = _matchingContext.SourceIndex - _sourceIndex;

			return new Match<TRaw>(_matchingContext, product, _index, matchLength, _sourceIndex, sourceLength, _sourceLine, _sourceLineIndex);
		}
	}
}
