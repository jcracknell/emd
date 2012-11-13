using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions {
	public enum ExpressionKind {
		ArrayLiteral,
		BooleanLiteral,
		Call,
		DocumentLiteral,
		DynamicProperty,
		Identifier,
		NullLiteral,
		NumericLiteral,
		ObjectLiteral,
		StaticProperty,
		StringLiteral,
		UriLiteral
	}
}
