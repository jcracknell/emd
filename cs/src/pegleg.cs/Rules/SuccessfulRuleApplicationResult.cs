using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Rules {
	class SuccessfulRuleApplicationResult<TProduct> : IRuleApplicationResult<TProduct> {
		public bool Success { get { return true; } }
		public TProduct Product { get; private set; }

		public SuccessfulRuleApplicationResult(TProduct product) {
			Product = product;
		}
	}
}
