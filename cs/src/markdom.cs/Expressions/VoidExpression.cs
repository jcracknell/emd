﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions {
	public class VoidExpression : UnaryExpression {
		public VoidExpression(IExpression body, SourceRange sourceRange)
			: base(ExpressionKind.Void, body, sourceRange) { }

		public override void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
