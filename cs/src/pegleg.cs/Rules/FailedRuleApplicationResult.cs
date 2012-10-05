using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Rules {
	public class FailedRuleApplicationResult<TProduct> : IRuleApplicationResult<TProduct> {
		public bool Success { get { return false; } }

		public TProduct Product {
			get { throw new NotImplementedException(); }
		}
	}
}
