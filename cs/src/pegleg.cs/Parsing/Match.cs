using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public class Match<TProduct> : IMatch<TProduct> {
		private readonly MatchingContext _context;
		private readonly IParsingExpression _expression;
		private readonly TProduct _product;
		private readonly int _index;
		private readonly int _length;
		private readonly SourceRange _matchRange;

		public Match(MatchingContext context, IParsingExpression expression, TProduct product, int index, int length, SourceRange matchRange) {
			CodeContract.ArgumentIsNotNull(() => context, context);
			CodeContract.ArgumentIsNotNull(() => expression, expression);
			CodeContract.ArgumentIsValid(() => index, index >= 0, "must be a non-negative integer");
			CodeContract.ArgumentIsValid(() => length, length >= 0, "must be a non-negative integer");

			_context = context;
			_expression = expression;
			_product = product;
			_index = index;
			_length = length;
			_matchRange = matchRange;
		}

		public SourceRange SourceRange { get { return _matchRange; } }

		public int Index { get { return _matchRange.Index; } }

		public int Length { get { return _matchRange.Length; } }

		public TProduct Product { get { return _product; } }

		public IParsingExpression Expression { get { return _expression; } }

		public string String { get { return _context.Substring(_index, _length); } }
	}
}
