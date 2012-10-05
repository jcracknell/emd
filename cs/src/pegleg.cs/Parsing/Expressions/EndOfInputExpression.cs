using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface EndOfInputExpression : IExpression {
	}

	public class EndOfInputExpression<TProduct> : EndOfInputExpression, IExpression<TProduct> {
		private readonly Func<IExpressionMatch<Nil>, TProduct> _matchAction;

		public EndOfInputExpression(Func<IExpressionMatch<Nil>, TProduct> matchAction) {
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_matchAction = matchAction;
		}

		public ExpressionType ExpressionType { get { return ExpressionType.EndOfInput; } }

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}

		public IExpressionMatchingResult Match(IExpressionMatchingContext context) {
			if(!context.AtEndOfInput)
				return new UnsuccessfulExpressionMatchingResult();

			var product = _matchAction(context.StartMatch().CompleteMatch(this, Nil.Value));

			return new SuccessfulExpressionMatchingResult(product);
		}
	}
}
