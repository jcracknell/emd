﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Model.Expressions {
	public class StringExpression : IExpression {
		private readonly string _value;
		private readonly MarkdomSourceRange _sourceRange;

		public StringExpression(string value, MarkdomSourceRange sourceRange) {
			_value = value;
			_sourceRange = sourceRange;
		}

		public string Value { get { return _value; } }

		public ExpressionKind Kind { get { return ExpressionKind.String; } }

		public MarkdomSourceRange SourceRange { get { return _sourceRange; } }

		public void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
