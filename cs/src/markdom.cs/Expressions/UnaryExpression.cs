﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions {
	public abstract class UnaryExpression : Expression {
		private readonly IExpression _body;

		public UnaryExpression(ExpressionKind kind, IExpression body, SourceRange sourceRange)
			: base(kind, sourceRange)
		{
			CodeContract.ArgumentIsNotNull(() => body, body);

			_body = body;
		}

		public IExpression Body { get { return _body; } }
	}
}
