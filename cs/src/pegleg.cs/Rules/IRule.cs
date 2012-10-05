using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Rules {
	public interface IRule<out TProduct> {
		IRuleApplicationResult<TProduct> Apply(IRuleApplicationContext context);
	}
}
