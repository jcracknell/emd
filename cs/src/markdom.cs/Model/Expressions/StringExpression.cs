using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Expressions {
	public class StringExpression : Expression {
		private readonly string _value;

		public StringExpression(string value, MarkdomSourceRange sourceRange)
			: base(sourceRange)
		{
			_value = value;
		}

		public string Value { get { return _value; } }

		public override ExpressionKind Kind { get { return ExpressionKind.String; } }

		public override bool Equals(object obj) {
			var other = obj as StringExpression;
			return null != other
				&& this.Value.Equals(other.Value)
				&& this.SourceRange.Equals(other.SourceRange);
		}
	}
}
