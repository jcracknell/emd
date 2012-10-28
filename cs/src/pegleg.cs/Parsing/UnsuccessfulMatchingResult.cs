using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public class UnsuccessfulMatchingResult : IMatchingResult {

		public bool Succeeded { get { return false; } }

		public object Product {
			get {
				throw new InvalidOperationException("Attempt to access Product of " + typeof(UnsuccessfulMatchingResult).Name);
			}
		}
	}
}
