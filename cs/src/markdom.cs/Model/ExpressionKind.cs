using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model {
	public enum ExpressionKind {
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
