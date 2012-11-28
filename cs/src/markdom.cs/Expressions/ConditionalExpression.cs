﻿using pegleg.cs.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace markdom.cs.Expressions {
	public class ConditionalExpression : Expression {
		private readonly IExpression _condition;
		private readonly IExpression _trueExpression;
		private readonly IExpression _falseExpression;

		public ConditionalExpression(IExpression condition, IExpression trueExpression, IExpression falseExpression, SourceRange sourceRange)
			: base(sourceRange)
		{
			CodeContract.ArgumentIsNotNull(() => condition, condition);
			CodeContract.ArgumentIsNotNull(() => trueExpression, trueExpression);
			CodeContract.ArgumentIsNotNull(() => falseExpression, falseExpression);

			_condition = condition;
			_trueExpression = trueExpression;
			_falseExpression = falseExpression;
		}

		public IExpression Condition { get { return _condition; } }

		public IExpression TrueExpression { get { return _trueExpression; } }

		public IExpression FalseExpression { get { return _falseExpression; } }

		public override void HandleWith(IExpressionHandler handler) {
			handler.Handle(this);
		}

		public override T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}