using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing {
	public interface IExpressionMatchBuilder {
		IExpressionMatch<TProduct> CompleteMatch<TProduct>(IExpression expression, TProduct product);
	}
}
