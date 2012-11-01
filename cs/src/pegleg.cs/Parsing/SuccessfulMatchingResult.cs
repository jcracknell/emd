using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public class SuccessfulMatchingResult : IMatchingResult {
		private readonly object _product;
		private static readonly SuccessfulMatchingResult _nilProduct;

		static SuccessfulMatchingResult() {
			_nilProduct = new SuccessfulMatchingResult(Nil.Value);
		}

		public SuccessfulMatchingResult(object product) {
			_product = product;
		}

		public bool Succeeded { get { return true; } }

		public object Product { get { return _product; } }

		public static SuccessfulMatchingResult NilProduct { get { return _nilProduct; } }
	}
}
