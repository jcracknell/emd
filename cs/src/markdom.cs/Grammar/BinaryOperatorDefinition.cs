using markdom.cs.Expressions;
using pegleg.cs;
using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Grammar {
	public class BinaryOperatorDefinition {
		public readonly IParsingExpression<object> Operator;
		public readonly Func<IExpression, IExpression, SourceRange, IExpression> Constructor;

		public BinaryOperatorDefinition(IParsingExpression<object> @operator, Func<IExpression, IExpression, SourceRange, IExpression> constructor) {
			Operator = @operator;
			Constructor = constructor;
		}
	}
}
