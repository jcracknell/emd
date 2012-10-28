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
		private readonly Func<IExpressionMatch<Nil>, TProduct> _noMatchAction;

		public OptionalExpression(IExpression body, Func<IExpressionMatch<object>, TProduct> matchAction, Func<IExpressionMatch<Nil>, TProduct> noMatchAction) {
			CodeContract.ArgumentIsNotNull(() => body, body);

			_body = body;
			_matchAction = matchAction;
			_noMatchAction = noMatchAction;
		}

		public IExpressionMatchingResult Match(IExpressionMatchingContext context) {
			var matchBuilder = context.StartMatch();

			var bodyApplicationContext = context.Clone();
			var bodyMatchResult = _body.Match(bodyApplicationContext);

			if(!bodyMatchResult.Succeeded)
				return new SuccessfulExpressionMatchingResult(
					null != _noMatchAction
						? _noMatchAction(matchBuilder.CompleteMatch(this, Nil.Value))
						: default(TProduct));

			context.Assimilate(bodyApplicationContext);

			return new SuccessfulExpressionMatchingResult(
				null != _matchAction
					? _matchAction(matchBuilder.CompleteMatch(this, bodyMatchResult.Product))
					: bodyMatchResult.Product);
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
