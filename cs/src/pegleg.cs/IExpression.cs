using pegleg.cs.Parsing;
using pegleg.cs.Parsing.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs {
	public interface IExpression {
		ExpressionType ExpressionType { get; }
		T HandleWith<T>(IExpressionHandler<T> handler);
		IExpressionMatchingResult Match(IExpressionMatchingContext context);
	}

	public interface IExpression<out TProduct> : IExpression {
	}
}
