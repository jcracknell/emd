using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emd.cs.Expressions {
	public class StrictEqualsExpression : BinaryExpression {
		public StrictEqualsExpression(IExpression left, IExpression right, SourceRange sourceRange)
			: base(left, right, sourceRange) { }

		public override void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
