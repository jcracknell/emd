using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model {
	public enum ExpressionKind {
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
