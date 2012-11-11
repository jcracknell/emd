using markdom.cs.Model.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model {
	public interface IExpressionHandler {
		void Handle(BooleanLiteralExpression expression);
		void Handle(CallExpression expression);
		void Handle(DocumentLiteralExpression expression);
		void Handle(IdentifierExpression expression);
		void Handle(IndexerExpression expression);
		void Handle(MemberExpression expression);
		void Handle(NullLiteralExpression expression);
		void Handle(NumericLiteralExpression expression);
		void Handle(ObjectLiteralExpression expression);
		void Handle(StringLiteralExpression expression);
		void Handle(UriLiteralExpression expression);
	}

	public interface IExpressionHandler<T> {
		T Handle(BooleanLiteralExpression expression);
		T Handle(CallExpression expression);
		T Handle(DocumentLiteralExpression expression);
		T Handle(IdentifierExpression expression);
		T Handle(IndexerExpression expression);
		T Handle(MemberExpression expression);
		T Handle(NullLiteralExpression expression);
		T Handle(NumericLiteralExpression expression);
		T Handle(ObjectLiteralExpression expression);
		T Handle(StringLiteralExpression expression);
		T Handle(UriLiteralExpression expression);
	}
}
