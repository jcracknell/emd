﻿using markdom.cs.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions {
	public interface IExpressionHandler {
		void Handle(ArrayLiteralExpression expression);
		void Handle(BooleanLiteralExpression expression);
		void Handle(CallExpression expression);
		void Handle(DocumentLiteralExpression expression);
		void Handle(IdentifierExpression expression);
		void Handle(DynamicPropertyExpression expression);
		void Handle(StaticPropertyExpression expression);
		void Handle(NullLiteralExpression expression);
		void Handle(NumericLiteralExpression expression);
		void Handle(ObjectLiteralExpression expression);
		void Handle(StringLiteralExpression expression);
		void Handle(UriLiteralExpression expression);
	}

	public interface IExpressionHandler<T> {
		T Handle(ArrayLiteralExpression expression);
		T Handle(BooleanLiteralExpression expression);
		T Handle(CallExpression expression);
		T Handle(DocumentLiteralExpression expression);
		T Handle(IdentifierExpression expression);
		T Handle(DynamicPropertyExpression expression);
		T Handle(StaticPropertyExpression expression);
		T Handle(NullLiteralExpression expression);
		T Handle(NumericLiteralExpression expression);
		T Handle(ObjectLiteralExpression expression);
		T Handle(StringLiteralExpression expression);
		T Handle(UriLiteralExpression expression);
	}
}
