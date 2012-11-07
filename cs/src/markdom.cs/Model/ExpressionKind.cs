using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model {
	public enum ExpressionKind {
		At,
		BooleanLiteral,
		DocumentLiteral,
		Identifier,
		NullLiteral,
		NumericLiteral,
		ObjectLiteral,
		StringLiteral,
		UriLiteral
	}
}
