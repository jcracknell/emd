﻿using markdom.cs.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions {
	public interface IExpressionHandler {
		void Handle(AdditionExpression expression);
		void Handle(ArrayLiteralExpression expression);
		void Handle(BitwiseAndExpression expression);
		void Handle(BitwiseNotExpression expression);
		void Handle(BitwiseOrExpression expression);
		void Handle(BitwiseXOrExpression expression);
		void Handle(BooleanLiteralExpression expression);
		void Handle(CallExpression expression);
		void Handle(DeleteExpression expression);
		void Handle(DivisionExpression expression);
		void Handle(DocumentLiteralExpression expression);
		void Handle(DynamicPropertyExpression expression);
		void Handle(EqualsExpression expression);
		void Handle(GreaterThanExpression expression);
		void Handle(GreaterThanOrEqualToExpression expression);
		void Handle(IdentifierExpression expression);
		void Handle(InExpression expression);
		void Handle(InstanceOfExpression expression);
		void Handle(LeftShiftExpression expression);
		void Handle(LessThanExpression expression);
		void Handle(LessThanOrEqualToExpression expression);
		void Handle(LogicalAndExpression expression);
		void Handle(LogicalNotExpression expression);
		void Handle(LogicalOrExpression expression);
		void Handle(ModuloExpression expression);
		void Handle(MultiplicationExpression expression);
		void Handle(NegativeExpression expression);
		void Handle(NotEqualsExpression expression);
		void Handle(NullLiteralExpression expression);
		void Handle(NumericLiteralExpression expression);
		void Handle(ObjectLiteralExpression expression);
		void Handle(PositiveExpression expression);
		void Handle(PostfixDecrementExpression expression);
		void Handle(PostfixIncrementExpression expression);
		void Handle(PrefixDecrementExpression expression);
		void Handle(PrefixIncrementExpression expression);
		void Handle(RightShiftExpression expression);
		void Handle(StaticPropertyExpression expression);
		void Handle(StrictEqualsExpression expression);
		void Handle(StrictNotEqualsExpression expression);
		void Handle(StringLiteralExpression expression);
		void Handle(SubtractionExpression expression);
		void Handle(TypeofExpression expression);
		void Handle(UnsignedRightShiftExpression expression);
		void Handle(UriLiteralExpression expression);
		void Handle(VoidExpression expression);
	}

	public interface IExpressionHandler<T> {
		T Handle(AdditionExpression expression);
		T Handle(ArrayLiteralExpression expression);
		T Handle(BitwiseAndExpression expression);
		T Handle(BitwiseNotExpression expression);
		T Handle(BitwiseOrExpression expression);
		T Handle(BitwiseXOrExpression expression);
		T Handle(BooleanLiteralExpression expression);
		T Handle(CallExpression expression);
		T Handle(DeleteExpression expression);
		T Handle(DivisionExpression expression);
		T Handle(DocumentLiteralExpression expression);
		T Handle(DynamicPropertyExpression expression);
		T Handle(EqualsExpression expression);
		T Handle(GreaterThanExpression expression);
		T Handle(GreaterThanOrEqualToExpression expression);
		T Handle(IdentifierExpression expression);
		T Handle(InExpression expression);
		T Handle(InstanceOfExpression expression);
		T Handle(LeftShiftExpression expression);
		T Handle(LessThanExpression expression);
		T Handle(LessThanOrEqualToExpression expression);
		T Handle(LogicalAndExpression expression);
		T Handle(LogicalNotExpression expression);
		T Handle(LogicalOrExpression expression);
		T Handle(ModuloExpression expression);
		T Handle(MultiplicationExpression expression);
		T Handle(NegativeExpression expression);
		T Handle(NotEqualsExpression expression);
		T Handle(NullLiteralExpression expression);
		T Handle(NumericLiteralExpression expression);
		T Handle(ObjectLiteralExpression expression);
		T Handle(PositiveExpression expression);
		T Handle(PostfixDecrementExpression expression);
		T Handle(PostfixIncrementExpression expression);
		T Handle(PrefixDecrementExpression expression);
		T Handle(PrefixIncrementExpression expression);
		T Handle(RightShiftExpression expression);
		T Handle(StaticPropertyExpression expression);
		T Handle(StrictEqualsExpression expression);
		T Handle(StrictNotEqualsExpression expression);
		T Handle(StringLiteralExpression expression);
		T Handle(SubtractionExpression expression);
		T Handle(TypeofExpression expression);
		T Handle(UnsignedRightShiftExpression expression);
		T Handle(UriLiteralExpression expression);
		T Handle(VoidExpression expression);
	}
}
