using pegleg.cs.Parsing;
using pegleg.cs.Parsing.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs {
	public interface IParsingExpression {
		Guid Id { get; }
		ParsingExpressionKind Kind { get; }
		T HandleWith<T>(IParsingExpressionHandler<T> handler);
		IMatchingResult Matches(IMatchingContext context);
	}

	public interface IParsingExpression<out TProduct> : IParsingExpression {
	}
}
