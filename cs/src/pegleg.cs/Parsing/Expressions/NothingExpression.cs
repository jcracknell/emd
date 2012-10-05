using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pegleg.cs.Parsing.Expressions {
	public interface NothingExpression : IExpression {
	}

	public class NothingExpression<TProduct> : NothingExpression, IExpression<TProduct> {
		private readonly Func<IExpressionMatch<Nil>, TProduct> _matchAction;

		public NothingExpression(Func<IExpressionMatch<Nil>, TProduct> matchAction)
		{
			CodeContract.ArgumentIsNotNull(() => matchAction, matchAction);

			_matchAction = matchAction;
		}

		public IExpressionMatchingResult Match(IExpressionMatchingContext context) {
			var product = _matchAction(context.StartMatch().CompleteMatch(this, Nil.Value));

			return new SuccessfulExpressionMatchingResult(product);
		}

		public ExpressionType ExpressionType { get { return ExpressionType.Nothing; } }

		public override string ToString() {
			return this.HandleWith(new BackusNaurishExpressionHandler());
		}

		public T HandleWith<T>(IExpressionHandler<T> handler) {
			return handler.Handle(this);
		}
	}
}
