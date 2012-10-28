using markdom.cs.Model.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model {
	public interface IExpressionHandler {
		void Handle(ObjectExpression expression);
		void Handle(StringExpression expression);
		void Handle(UriExpression expression);
	}

	public interface IExpressionHandler<T> {
		T Handle(ObjectExpression expression);
		T Handle(StringExpression expression);
		T Handle(UriExpression expression);
	}
}
