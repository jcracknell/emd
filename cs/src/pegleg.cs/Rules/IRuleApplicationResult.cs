using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Rules {
	public interface IRuleApplicationResult<out TProduct> {
		bool Success { get; }
		TProduct Product { get; }
	}
}
