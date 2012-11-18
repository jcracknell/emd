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

		public IMatchingResult<TProduct> Success(TProduct product) {
			return new SuccessfulMatchingResult<TProduct>(product);
		}

		public IMatchingResult<TProduct> Success<TRaw>(TRaw raw, Func<IMatch<TRaw>, TProduct> matchAction) {
			var matchLength = _matchingContext.Index - _index;
			var sourceLength = _matchingContext.SourceIndex - _sourceIndex;
			var match = new Match<TRaw>(_matchingContext, raw, _index, matchLength, _sourceIndex, sourceLength, _sourceLine, _sourceLineIndex);
			
			var product = matchAction(match);
			return new SuccessfulMatchingResult<TProduct>(product);
		}
	}
}
