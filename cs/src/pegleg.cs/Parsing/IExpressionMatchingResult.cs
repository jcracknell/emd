using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public interface IExpressionMatchingResult {
		bool Succeeded { get; }
		object Product { get; }
	}
}
