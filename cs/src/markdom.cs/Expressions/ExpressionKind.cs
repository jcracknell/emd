using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions {
	public enum ExpressionKind {
		ArrayLiteral,
		BitwiseNot,
		BooleanLiteral,
		Call,
		Delete,
		DocumentLiteral,
		DynamicProperty,
		Identifier,
		LogicalNot,
		Negative,
		NullLiteral,
		NumericLiteral,
		ObjectLiteral,
		Positive,
		PostfixDecrement,
		PostfixIncrement,
		PrefixDecrement,
		PrefixIncrement,
		StaticProperty,
		StringLiteral,
		Typeof,
		UriLiteral,
		Void
	}
}
