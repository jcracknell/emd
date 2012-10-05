using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface DynamicExpression : IExpression {
		IExpression ConstructExpression();
	}

	public class DynamicExpression<TProduct> : DynamicExpression, IExpression<TProduct> {
		private readonly Func<IExpression<TProduct>> _expression;

		public DynamicExpression(Func<IExpression<TProduct>> expression) {
			CodeContract.ArgumentIsNotNull(() => expression, expression);

			_expression = expression;
		}

		public IExpression ConstructExpression() {
			return _expression();
		}

		public ExpressionType ExpressionType { get { return ExpressionType.Closure; } }

		public IExpressionMatchingResult Match(IExpressionMatchingContext context) {
			var closed = _expression();
			return closed.Match(context);
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}
	}
}
