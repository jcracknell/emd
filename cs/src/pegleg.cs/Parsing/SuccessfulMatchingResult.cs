using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public static class SuccessfulMatchingResult {
		private static readonly SuccessfulMatchingResult<Nil> _nilProduct;

		static SuccessfulMatchingResult() {
			_nilProduct = new SuccessfulMatchingResult<Nil>(Nil.Value);
		}

		public static SuccessfulMatchingResult<Nil> NilProduct { get { return _nilProduct; } }

		public static SuccessfulMatchingResult<TProduct> Create<TProduct>(TProduct product) {
			return new SuccessfulMatchingResult<TProduct>(product);
		}
	}

	public class SuccessfulMatchingResult<TProduct> : IMatchingResult<TProduct> {
		private readonly TProduct _product;

		public SuccessfulMatchingResult(TProduct product) {
			_product = product;
		}

		public bool Succeeded { get { return true; } }

		public TProduct Product { get { return _product; } }

		object IMatchingResult.Product { get { return _product; } }
	}
}
