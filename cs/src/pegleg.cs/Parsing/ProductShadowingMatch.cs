using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public class ProductShadowingMatch<TProduct> : IMatch<TProduct> {
		private readonly IMatch _shadowed;
		private readonly TProduct _product;

		public ProductShadowingMatch<TProduct> Create<TProduct>(IMatch match, TProduct product) {
			return new ProductShadowingMatch<TProduct>(match, product);
		}

		public ProductShadowingMatch(IMatch shadowed, TProduct product) {
			if(null == shadowed) throw ExceptionBecause.ArgumentNull(() => shadowed);

			_shadowed = shadowed;
			_product = product;
		}

		public TProduct Product { get { return _product; } }

		public SourceRange SourceRange { get { return _shadowed.SourceRange; } }

		public int Length { get { return _shadowed.Length; } }

		public string String { get { return _shadowed.String; } }
	}
}
