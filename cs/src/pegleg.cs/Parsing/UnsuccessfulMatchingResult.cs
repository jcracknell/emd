using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public static class UnsuccessfulMatchingResult {
		public static UnsuccessfulMatchingResult<TProduct> Create<TProduct>(IParsingExpression<TProduct> expression) {
			// The argument here is obviously unused, but it does provide nice type inferral!
			return new UnsuccessfulMatchingResult<TProduct>();
		}
	}

	public class UnsuccessfulMatchingResult<TProduct> : IMatchingResult<TProduct> {
		public bool Succeeded { get { return false; } }

		public TProduct Product { get { throw new InvalidOperationException(); } }

		object IMatchingResult.Product { get { throw new InvalidOperationException(); } }
	}
}
