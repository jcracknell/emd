using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public class SuccessfulExpressionMatchingResult : IExpressionMatchingResult {
		private readonly object _product;

		public SuccessfulExpressionMatchingResult(object product) {
			_product = product;
		}

		public bool Succeeded { get { return true; } }

		public object Product { get { return _product; } }
	}
}
