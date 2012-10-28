using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Expressions {
	public class UriExpression : Expression {
		private readonly string _value;

		public UriExpression(string value, MarkdomSourceRange sourceRange)
			: base(sourceRange)
		{
			_value = value;
		}

		public string Value { get { return _value; } }

		public override ExpressionKind Kind { get { return ExpressionKind.Uri; } }

		public override bool Equals(object obj) {
			var other = obj as UriExpression;
			return null != other
				&& this.Value.Equals(other.Value)
				&& this.SourceRange.Equals(other.SourceRange);
		}
	}
}
