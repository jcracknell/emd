using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public interface IMatchBuilder {
		IMatch<TProduct> CompleteMatch<TProduct>(IParsingExpression expression, TProduct product);
	}
}
