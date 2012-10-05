using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface AheadExpression : IExpression {
		IExpression Body { get; }
	}

	public class AheadExpression<TProduct> : AheadExpression, IExpression<TProduct> {
		private readonly IExpression _body;
		private readonly Func<IExpressionMatch<object>, TProduct> _matchAction;

		public AheadExpression(IExpression body, Func<IExpressionMatch<object>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => body, body);
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);
		
			_body = body;	
			_matchAction = matchAction;
		}

		public IExpression Body { get { return _body; } }

		public ExpressionType ExpressionType { get { return ExpressionType.Lookahead; } }

		public IExpressionMatchingResult Match(IExpressionMatchingContext context) {
			var bodyMatchingContext = context.Clone();
			var bodyMatchingResult = _body.Match(bodyMatchingContext);

			if(bodyMatchingResult.Succeeded) {
				var product = _matchAction(context.StartMatch().CompleteMatch(this, bodyMatchingResult.Product));

				return new SuccessfulExpressionMatchingResult(product);
			} else {
				return new UnsuccessfulExpressionMatchingResult();
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
