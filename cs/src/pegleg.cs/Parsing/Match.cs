using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public class Match<TProduct> : IMatch<TProduct> {
		private readonly MatchingContext _context;
		private readonly TProduct _product;
		private readonly int _index;
		private readonly int _length;
		private readonly SourceRange _matchRange;

		public Match(MatchingContext context, TProduct product, int index, int length, SourceRange matchRange) {
			CodeContract.ArgumentIsNotNull(() => context, context);
			CodeContract.ArgumentIsValid(() => index, index >= 0, "must be a non-negative integer");
			CodeContract.ArgumentIsValid(() => length, length >= 0, "must be a non-negative integer");

			_context = context;
			_product = product;
			_index = index;
			_length = length;
			_matchRange = matchRange;
		}

		public SourceRange SourceRange { get { return _matchRange; } }

		public int Length { get { return _matchRange.Length; } }

		public TProduct Product { get { return _product; } }

		public string String { get { return _context.Substring(_index, _length); } }
	}
}
