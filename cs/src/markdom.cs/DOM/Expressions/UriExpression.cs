using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.DOM.Expressions {
	public class UriExpression : Expression {
		private readonly string _value;

		public UriExpression(string value, SourceRange sourceRange)
			: base(sourceRange)
		{
			_value = value;
		}

		public string Value { get { return _value; } }

		public override ExpressionType Type { get { return ExpressionType.Uri; } }

		public override bool Equals(object obj) {
			var other = obj as UriExpression;
			return null != other
				&& this.Value.Equals(other.Value)
				&& this.SourceRange.Equals(other.SourceRange);
		}
	}
}
