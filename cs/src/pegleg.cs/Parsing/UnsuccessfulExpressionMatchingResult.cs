using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public class UnsuccessfulExpressionMatchingResult : IExpressionMatchingResult {

		public bool Succeeded { get { return false; } }

		public object Product {
			get {
				throw new InvalidOperationException("Attempt to access Product of " + typeof(UnsuccessfulExpressionMatchingResult).Name);
			}
		}
	}
}
