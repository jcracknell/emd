using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions.Builders {
	public class DefaultExpressionNamingConvention : IExpressionNamingConvention {
		public string Apply(string name, IExpression expression) {
			return name;
		}
	}
}
