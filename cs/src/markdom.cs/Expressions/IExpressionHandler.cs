using markdom.cs.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions {
	public interface IExpressionHandler {
		void Handle(ArrayLiteralExpression expression);
		void Handle(BitwiseNotExpression expression);
		void Handle(BooleanLiteralExpression expression);
		void Handle(CallExpression expression);
		void Handle(DeleteExpression expression);
		void Handle(DocumentLiteralExpression expression);
		void Handle(IdentifierExpression expression);
		void Handle(LogicalNotExpression expression);
		void Handle(DynamicPropertyExpression expression);
		void Handle(StaticPropertyExpression expression);
		void Handle(NegativeExpression expression);
		void Handle(NullLiteralExpression expression);
		void Handle(NumericLiteralExpression expression);
		void Handle(ObjectLiteralExpression expression);
		void Handle(PositiveExpression expression);
		void Handle(PostfixDecrementExpression expression);
		void Handle(PostfixIncrementExpression expression);
		void Handle(PrefixDecrementExpression expression);
		void Handle(PrefixIncrementExpression expression);
		void Handle(StringLiteralExpression expression);
		void Handle(TypeofExpression expression);
		void Handle(UriLiteralExpression expression);
		void Handle(VoidExpression expression);
	}

	public interface IExpressionHandler<T> {
		T Handle(ArrayLiteralExpression expression);
		T Handle(BitwiseNotExpression expression);
		T Handle(BooleanLiteralExpression expression);
		T Handle(CallExpression expression);
		T Handle(DeleteExpression expression);
		T Handle(DocumentLiteralExpression expression);
		T Handle(IdentifierExpression expression);
		T Handle(LogicalNotExpression expression);
		T Handle(DynamicPropertyExpression expression);
		T Handle(StaticPropertyExpression expression);
		T Handle(NegativeExpression expression);
		T Handle(NullLiteralExpression expression);
		T Handle(NumericLiteralExpression expression);
		T Handle(ObjectLiteralExpression expression);
		T Handle(PositiveExpression expression);
		T Handle(PostfixDecrementExpression expression);
		T Handle(PostfixIncrementExpression expression);
		T Handle(PrefixDecrementExpression expression);
		T Handle(PrefixIncrementExpression expression);
		T Handle(StringLiteralExpression expression);
		T Handle(TypeofExpression expression);
		T Handle(UriLiteralExpression expression);
		T Handle(VoidExpression expression);
	}
}
