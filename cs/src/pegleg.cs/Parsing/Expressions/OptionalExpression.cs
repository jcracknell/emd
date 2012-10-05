using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface OptionalExpression : IExpression {
		IExpression Body { get; }
	}

	public class OptionalExpression<TProduct> : OptionalExpression, IExpression<TProduct> {
		private readonly IExpression _body;
		private readonly Func<IExpressionMatch<object>, TProduct> _matchAction;

		public OptionalExpression(IExpression body, Func<IExpressionMatch<object>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => body, body);
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_body = body;
			_matchAction = matchAction;
		}

		public IExpressionMatchingResult Match(IExpressionMatchingContext context) {
			var bodyApplicationContext = context.Clone();
			var bodyMatchResult = _body.Match(bodyApplicationContext);

			if(!bodyMatchResult.Succeeded)
				return new SuccessfulExpressionMatchingResult(null);

			var matchBuilder = context.StartMatch();
			context.Assimilate(bodyApplicationContext);

			var product = _matchAction(matchBuilder.CompleteMatch(this, bodyMatchResult.Product));

			return new SuccessfulExpressionMatchingResult(product);
		}

		public OptionalExpression(IExpression<object> body) {
			CodeContract.ArgumentIsNotNull(() => body, body);

			_body = body;
		}

		public IExpression Body { get { return _body; } }

		public ExpressionType ExpressionType { get { return ExpressionType.Optional; } }

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
