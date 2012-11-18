using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions {
	public interface IExpression {
		SourceRange SourceRange { get; }
		void HandleWith(IExpressionHandler handler);
		T HandleWith<T>(IExpressionHandler<T> handler);
	}
}
