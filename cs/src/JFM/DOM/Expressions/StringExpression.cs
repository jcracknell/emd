using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JFM.DOM.Expressions {
	public class StringExpression : Expression {
		private readonly string _value;

		public StringExpression(string value, SourceRange sourceRange)
			: base(sourceRange)
		{
			_value = value;
		}

		public string Value { get { return _value; } }

		public override ExpressionType Type { get { return ExpressionType.String; } }
	}
}
