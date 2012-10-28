using markdom.cs.Model.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model {
	public interface IExpressionHandler {
		void Handle(NumericLiteralExpression expression);
		void Handle(ObjectExpression expression);
		void Handle(StringLiteralExpression expression);
		void Handle(UriLiteralExpression expression);
	}

	public interface IExpressionHandler<T> {
		T Handle(NumericLiteralExpression expression);
		T Handle(ObjectExpression expression);
		T Handle(StringLiteralExpression expression);
		T Handle(UriLiteralExpression expression);
	}
}
