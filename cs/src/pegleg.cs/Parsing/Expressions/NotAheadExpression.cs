using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface NotAheadExpression : IExpression {
		IExpression Body { get; }
	}

	public class NotAheadExpression<TProduct> : NotAheadExpression, IExpression<TProduct> {
		private readonly IExpression _body;
		private readonly Func<IExpressionMatch<Nil>, TProduct> _matchAction;

		public NotAheadExpression(IExpression body, Func<IExpressionMatch<Nil>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => body, body);
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_body = body;
			_matchAction = matchAction;
		}

		public IExpression Body { get { return _body; } }

		public ExpressionType ExpressionType { get { return ExpressionType.NegativeLookahead; } }

		public IExpressionMatchingResult Match(IExpressionMatchingContext context) {
			var bodyMatchingContext = context.Clone();
			var bodyMatchingResult = _body.Match(bodyMatchingContext);

			if(bodyMatchingResult.Succeeded) {
				return new UnsuccessfulExpressionMatchingResult();
			} else {
				var product = _matchAction(context.StartMatch().CompleteMatch(this, Nil.Value));

				return new SuccessfulExpressionMatchingResult(product);
			}
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}
	}
}
